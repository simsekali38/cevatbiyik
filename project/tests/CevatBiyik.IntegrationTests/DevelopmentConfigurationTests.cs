using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;

public sealed class DevelopmentConfigurationTests
{
    [Fact]
    public async Task Development_LoadsLocalJsonWithoutLegacyUserSecrets_AndServesLocalhost()
    {
        using var factory = new PublicWebFactory();
        using var development = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development").ConfigureServices(services =>
            services.AddSingleton<IRepository<PrivateDocument>>(new PublicTestRepository<PrivateDocument>())));
        var configuration = (IConfigurationRoot)development.Services.GetRequiredService<IConfiguration>();
        var jsonSources = configuration.Providers.OfType<JsonConfigurationProvider>().ToArray();
        var localJson = Assert.Single(jsonSources, provider => provider.Source.Path == "appsettings.Development.json");
        Assert.True(localJson.TryGet("AllowedHosts", out var hosts));
        Assert.Contains("localhost", hosts, StringComparison.Ordinal);
        Assert.True(localJson.TryGet("Site:BaseUrl", out var origin));
        Assert.True(Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.IsLoopback);
        Assert.DoesNotContain(jsonSources, provider => provider.Source.Path == "secrets.json");
        Assert.DoesNotContain(typeof(PublicSiteSettings).Assembly.GetCustomAttributesData(),
            attribute => attribute.AttributeType.Name == "UserSecretsIdAttribute");
        using var client = development.CreateClient(new() { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        var response = await client.GetAsync("/");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("CONTENT_REQUIRED", html, StringComparison.Ordinal);
        Assert.Contains("Ana sayfa", html, StringComparison.Ordinal);
    }
}
