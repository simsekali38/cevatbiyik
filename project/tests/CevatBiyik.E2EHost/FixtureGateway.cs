using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using CevatBiyik.IntegrationTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

internal static class FixtureGateway
{
    public static async Task Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--demo") { await DemoCommand.RunAsync(args); return; }
        var cmsSeed = args.Length == 1 && args[0] == "--cms-seed-fixture";
        if (args.Length != 1 || (!cmsSeed && args[0] != "--fixture-only"))
        { throw new InvalidOperationException("Test-only loopback host requires --fixture-only."); }
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "CevatBiyik.sln"))) { root = root.Parent; }
        if (root is null) { throw new InvalidOperationException("Run the test fixture from its repository checkout."); }
        using var factory = new AdminWebFactory { FixtureContentRoot = Path.Combine(root.FullName, "src", "CevatBiyik.Web") };
        await factory.InitializeAsync();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (cmsSeed) { await CmsSeedFixture.LoadIntoSqliteFixtureAsync(db); }
            else
            {
            db.Articles.Add(new Article { Title = "TEST_ONLY browser article", Slug = "test-browser", Content = "TEST_ONLY browser content", Excerpt = "TEST_ONLY search", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1) });
            db.PracticeAreas.Add(new PracticeArea { Name = "TEST_ONLY practice area", Slug = "test-area", Content = "TEST_ONLY area", IsActive = true });
            db.FaqItems.Add(new FaqItem { Question = "TEST_ONLY question?", Answer = "TEST_ONLY answer", IsActive = true, IsReviewed = true });
            foreach (var path in new[] { "/kvkk", "/cerez-politikasi" })
            { db.PageContents.Add(new PageContent { Path = path, Title = "TEST_ONLY legal page", Body = "TEST_ONLY reviewed legal fixture", IsReviewed = true, IsActive = true }); }
            await db.SaveChangesAsync();
            }
        }
        // Only synthetic test configuration and repositories; no root config, user-secrets, SMTP or real MySQL.
        using var configured = factory.WithWebHostBuilder(b => { b.ConfigureServices(s => s.AddSingleton<IStartupFilter, E2EConnectionFilter>()); b.ConfigureAppConfiguration((_, c) => c.AddInMemoryCollection(new Dictionary<string, string?>
        { ["Analytics:Enabled"] = cmsSeed ? "false" : "true", ["Analytics:LegalReviewConfirmed"] = "true", ["Analytics:GtmId"] = "GTM-TEST123",
          ["Analytics:Ga4Id"] = "", ["Analytics:ConsentVersion"] = "e2e-v1", ["Analytics:Notice"] = "TEST_ONLY approved analytics fixture notice." })); });
        using var upstream = configured.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost:57413"), AllowAutoRedirect = false, HandleCookies = false });
        using var key = RSA.Create(2048);
        var certRequest = new CertificateRequest("CN=localhost", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var names = new SubjectAlternativeNameBuilder(); names.AddDnsName("localhost"); names.AddIpAddress(IPAddress.Loopback);
        certRequest.CertificateExtensions.Add(names.Build());
        using var generated = certRequest.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
        // Windows Schannel requires an imported key; do not install or trust a certificate in any store.
        using var certificate = new X509Certificate2(generated.Export(X509ContentType.Pfx));
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { Args = [], EnvironmentName = "Testing", ContentRootPath = AppContext.BaseDirectory });
        builder.Configuration.Sources.Clear();
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(k => k.Listen(IPAddress.Loopback, 57413, o => o.UseHttps(certificate)));
        await using var app = builder.Build();
        app.Run(async context =>
        {
            if (!IPAddress.IsLoopback(context.Connection.RemoteIpAddress!) || context.Request.Host.Host is not ("localhost" or "127.0.0.1"))
            { context.Response.StatusCode = 403; return; }
            using var request = new HttpRequestMessage(new HttpMethod(context.Request.Method), context.Request.Path + context.Request.QueryString);
            if (context.Request.ContentLength > 0 || context.Request.Headers.ContainsKey("Transfer-Encoding"))
            { request.Content = new StreamContent(context.Request.Body); }
            foreach (var header in context.Request.Headers)
            {
                if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase)) { continue; }
                if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                { request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()); }
            }
            using var response = await upstream.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
            context.Response.StatusCode = (int)response.StatusCode;
            foreach (var header in response.Headers.Concat(response.Content.Headers))
            { if (header.Key is not ("Transfer-Encoding" or "Connection")) { context.Response.Headers[header.Key] = header.Value.ToArray(); } }
            context.Response.Headers["X-Test-Fixture"] = cmsSeed ? "isolated-cms-seed" : "isolated-phase13";
            await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
        });
        Console.WriteLine("Isolated E2E fixture: https://localhost:57413 (temporary SQLite, fake scanner, no external providers).");
        await app.RunAsync();
    }
}

internal sealed class E2EConnectionFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
    {
        app.Use((context, following) =>
        {
            var value = context.Request.Headers["X-Test-Connection-IP"].ToString();
            if (IPAddress.TryParse(value, out var address) && value.StartsWith("198.18.0.", StringComparison.Ordinal))
            { context.Connection.RemoteIpAddress = address; }
            return following(context);
        });
        next(app);
    };
}
