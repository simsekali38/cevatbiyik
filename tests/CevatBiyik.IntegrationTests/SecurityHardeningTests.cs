using System.Net;
using CevatBiyik.Infrastructure.Media;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;

public sealed class SecurityHardeningTests
{
    private static void Headers(HttpResponseMessage response)
    {
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("strict-origin-when-cross-origin", response.Headers.GetValues("Referrer-Policy").Single());
        Assert.Contains("geolocation=()", response.Headers.GetValues("Permissions-Policy").Single(), StringComparison.Ordinal);
        var csp = response.Headers.GetValues("Content-Security-Policy").Single();
        Assert.Contains("frame-ancestors 'none'", csp, StringComparison.Ordinal);
        Assert.Contains("frame-src 'none'", csp, StringComparison.Ordinal);
        Assert.Contains("'nonce-", csp, StringComparison.Ordinal);
        Assert.DoesNotContain("unsafe-inline", csp, StringComparison.Ordinal);
        Assert.DoesNotContain("unsafe-eval", csp, StringComparison.Ordinal);
        Assert.DoesNotContain("googletagmanager", csp, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("/", 200)]
    [InlineData("/css/site.css", 200)]
    [InlineData("/missing-page", 404)]
    [InlineData("/uploads/attack.aspx", 404)]
    [InlineData("/UPLOADS/web.config", 404)]
    [InlineData("/uploads/images/aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.webp%0A", 404)]
    [InlineData("/admin/documents/download/1", 302)]
    public async Task Headers_ProtectHtmlStaticDeniedUploadsAndAuthorizationRedirects(string path, int status)
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        using var response = await client.GetAsync(path);
        Assert.Equal(status, (int)response.StatusCode);
        Headers(response);
    }

    [Fact]
    public async Task Nonces_AreUnpredictableAndFreshPerResponse()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        using var first = await client.GetAsync("/");
        using var second = await client.GetAsync("/");
        Assert.NotEqual(first.Headers.GetValues("Content-Security-Policy").Single(), second.Headers.GetValues("Content-Security-Policy").Single());
    }

    [Fact]
    public async Task HttpRedirect_HasHeaders_AndHstsOnlyOnNonLocalHttps()
    {
        using var factory = new PublicWebFactory();
        using var http = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("http://example.test"), AllowAutoRedirect = false });
        using var redirect = await http.GetAsync("/");
        Assert.Equal(HttpStatusCode.TemporaryRedirect, redirect.StatusCode);
        Headers(redirect);
        Assert.False(redirect.Headers.Contains("Strict-Transport-Security"));
        using var https = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://example.test"), AllowAutoRedirect = false });
        using var response = await https.GetAsync("/");
        Assert.Equal("max-age=15552000", response.Headers.GetValues("Strict-Transport-Security").Single());
    }

    [Fact]
    public async Task StaticLimiter_CannotBeBypassedWithSpoofedForwardedIp_AndReturnsHardened429()
    {
        using var factory = new PublicWebFactory();
        using var configured = factory.WithWebHostBuilder(builder => builder.ConfigureServices(services => services.AddSingleton<IStartupFilter>(new ConnectionFilter("203.0.113.9"))));
        using var client = configured.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        for (var i = 0; i < 120; i++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/css/site.css");
            request.Headers.Add("X-Forwarded-For", "198.51.100." + (i + 1));
            request.Headers.Add("X-Forwarded-Proto", "https");
            using var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        using var blocked = await client.GetAsync("/css/site.css");
        Assert.Equal(HttpStatusCode.TooManyRequests, blocked.StatusCode);
        Assert.Equal("60", blocked.Headers.GetValues("Retry-After").Single());
        Headers(blocked);
    }

    [Theory]
    [InlineData(false, 307)]
    [InlineData(true, 200)]
    public async Task OnlyExplicitTrustedProxy_CanChangeScheme(bool trusted, int status)
    {
        using var factory = new PublicWebFactory();
        using var configured = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services => services.AddSingleton<IStartupFilter>(new ConnectionFilter("203.0.113.9")));
            if (trusted) { builder.ConfigureAppConfiguration((_, c) => c.AddInMemoryCollection(new Dictionary<string, string?> { ["Security:TrustedProxies:0"] = "203.0.113.9" })); }
        });
        using var client = configured.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("http://example.test"), AllowAutoRedirect = false });
        using var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("X-Forwarded-For", "198.51.100.1");
        request.Headers.Add("X-Forwarded-Proto", "https");
        request.Headers.Add("X-Forwarded-Host", "attacker.example");
        using var response = await client.SendAsync(request);
        Assert.Equal(status, (int)response.StatusCode);
        Assert.DoesNotContain("attacker.example", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Headers(response);
    }

    [Fact]
    public async Task LoginLimiter_CountsInvalidCsrfAttempts_AndNormalizesIpv4MappedAddresses()
    {
        using var factory = new PublicWebFactory();
        using var configured = factory.WithWebHostBuilder(builder => builder.ConfigureServices(services => services.AddSingleton<IStartupFilter>(new ConnectionFilter("127.0.0.1"))));
        using var client = configured.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        for (var i = 0; i < 10; i++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "/admin/login") { Content = new FormUrlEncodedContent(new Dictionary<string, string>()) };
            request.Headers.Add("X-Forwarded-For", i % 2 == 0 ? "198.51.100.1" : "::ffff:198.51.100.1");
            request.Headers.Add("X-Forwarded-Proto", "https");
            using var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        using var blockedRequest = new HttpRequestMessage(HttpMethod.Post, "/admin/login") { Content = new FormUrlEncodedContent(new Dictionary<string, string>()) };
        blockedRequest.Headers.Add("X-Forwarded-For", "198.51.100.1");
        blockedRequest.Headers.Add("X-Forwarded-Proto", "https");
        using var blocked = await client.SendAsync(blockedRequest);
        Assert.Equal(HttpStatusCode.TooManyRequests, blocked.StatusCode);
        Headers(blocked);
    }

    [Theory]
    [InlineData("attack.exe", "image/png")]
    [InlineData("attack.js", "image/png")]
    [InlineData("image.png:payload.exe", "image/png")]
    [InlineData("image.png", "image/jpeg")]
    public async Task ImageAttack_RejectedBeforeStorage(string name, string mime)
    {
        var root = Path.Combine(Path.GetTempPath(), "cevat-attack-" + Guid.NewGuid().ToString("N"));
        using var storage = new FileImageStorage(root);
        using var content = new MemoryStream(ImageStorageTests.Png(10, 10));
        await Assert.ThrowsAsync<ValidationException>(() => storage.StoreAsync(content, name, mime));
        Assert.False(Directory.Exists(root));
    }

    [Fact]
    public async Task PixelProductBomb_IsRejectedEvenWhenEachDimensionIsAllowed()
    {
        var root = Path.Combine(Path.GetTempPath(), "cevat-attack-" + Guid.NewGuid().ToString("N"));
        using var storage = new FileImageStorage(root);
        using var content = new MemoryStream(ImageStorageTests.Png(4001, 4000));
        await Assert.ThrowsAsync<ValidationException>(() => storage.StoreAsync(content, "bomb.png", "image/png"));
        Assert.False(Directory.Exists(root));
    }

    [Theory]
    [InlineData("/admin")]
    [InlineData("/admin/articles")]
    [InlineData("/admin/practice-areas")]
    [InlineData("/admin/contact-messages")]
    [InlineData("/admin/settings")]
    [InlineData("/admin/documents")]
    [InlineData("/admin/documents/download/1")]
    [InlineData("/admin/files")]
    [InlineData("/admin/media")]
    [InlineData("/admin/redirects")]
    [InlineData("/admin/locations")]
    public async Task EveryAdminModule_RejectsAnonymousRequests(string path)
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        using var response = await client.GetAsync(path);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("https://localhost/admin/login", response.Headers.Location!.ToString(), StringComparison.Ordinal);
        Assert.True(response.Headers.CacheControl!.NoStore);
    }

    [Theory]
    [InlineData("0.0.0.0")]
    [InlineData("::")]
    [InlineData("203.0.113.0/24")]
    [InlineData("*")]
    public void InvalidProxyConfiguration_FailsClosed(string address)
    {
        using var factory = new PublicWebFactory();
        using var configured = factory.WithWebHostBuilder(builder => builder.ConfigureAppConfiguration((_, c) =>
            c.AddInMemoryCollection(new Dictionary<string, string?> { ["Security:TrustedProxies:0"] = address })));
        Assert.Throws<InvalidOperationException>(() => configured.CreateClient());
    }

    [Fact]
    public async Task ContactLimiter_CountsRejectedCsrfPostsWithoutCreatingMessages()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        for (var i = 0; i < 5; i++)
        {
            using var response = await client.PostAsync("/iletisim", new FormUrlEncodedContent(new Dictionary<string, string>()));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        using var blocked = await client.PostAsync("/iletisim", new FormUrlEncodedContent(new Dictionary<string, string>()));
        Assert.Equal(HttpStatusCode.TooManyRequests, blocked.StatusCode);
        Headers(blocked);
        Assert.Empty(factory.Messages.Items);
    }

    [Fact]
    public async Task AsymmetricForwardingHeaders_AreRejectedEvenFromTrustedProxy()
    {
        using var factory = new PublicWebFactory();
        using var configured = factory.WithWebHostBuilder(builder => builder.ConfigureServices(services => services.AddSingleton<IStartupFilter>(new ConnectionFilter("127.0.0.1"))));
        using var client = configured.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("http://example.test"), AllowAutoRedirect = false });
        using var request = new HttpRequestMessage(HttpMethod.Get, "/");
        request.Headers.Add("X-Forwarded-Proto", "https");
        using var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.TemporaryRedirect, response.StatusCode);
    }

    [Fact]
    public async Task AllAdminGetAndMutationRoutes_ChallengeAnonymousBeforeActionExecution()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var provider = factory.Services.GetRequiredService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider>();
        var actions = provider.ActionDescriptors.Items.OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
            .Where(x => x.RouteValues.TryGetValue("area", out var area) && area == "Admin").ToArray();
        Assert.True(actions.Length >= 40);
        foreach (var action in actions)
        {
            var template = action.AttributeRouteInfo!.Template!.TrimStart('/');
            if (action.EndpointMetadata.OfType<Microsoft.AspNetCore.Authorization.IAllowAnonymous>().Any())
            {
                Assert.True(template is "admin/login" or "admin/access-denied");
                continue;
            }
            var path = "/" + System.Text.RegularExpressions.Regex.Replace(template, @"\{[^}]+\}", "1");
            var methods = action.ActionConstraints!.OfType<Microsoft.AspNetCore.Mvc.ActionConstraints.HttpMethodActionConstraint>()
                .SelectMany(x => x.HttpMethods).ToArray();
            Assert.NotEmpty(methods);
            foreach (var method in methods)
            {
                using var request = new HttpRequestMessage(new HttpMethod(method), path);
                if (method == "POST") { request.Content = new FormUrlEncodedContent(new Dictionary<string, string>()); }
                using var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
                Assert.StartsWith("https://localhost/admin/login", response.Headers.Location!.ToString(), StringComparison.Ordinal);
            }
        }
    }

    private sealed class ConnectionFilter(string remoteIp) : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            app.Use((context, following) => { context.Connection.RemoteIpAddress = IPAddress.Parse(remoteIp); return following(context); });
            next(app);
        };
    }
}
