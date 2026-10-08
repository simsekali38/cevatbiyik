using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CevatBiyik.IntegrationTests;

/// <summary>
/// Faz 01 baseline smoke testleri: uygulama ayağa kalkar, ana sayfa döner, hata sayfası doğru çalışır.
/// </summary>
public class BaselineSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BaselineSmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task AnaSayfa_200DonerVeTurkceDilIcerir()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("lang=\"tr\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task AnaSayfa_VarsayilanOlarakIndexlenmez()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("noindex", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task OlmayanSayfa_404DonerVeHataSayfasiGosterir()
    {
        var response = await _client.GetAsync("/bu-sayfa-yok");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("404", html, StringComparison.Ordinal);
        Assert.True(response.Headers.Contains("X-Robots-Tag"));
    }
}
