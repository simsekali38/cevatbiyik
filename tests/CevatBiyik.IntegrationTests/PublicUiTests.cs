using System.Net;
using System.Text.RegularExpressions;

namespace CevatBiyik.IntegrationTests;

public partial class PublicUiTests(PublicWebFactory factory) : IClassFixture<PublicWebFactory>
{
    [Theory]
    [InlineData("/", "Av. Cevat")]
    [InlineData("/hakkimda", "Hakkımda")]
    [InlineData("/makaleler", "Makaleler")]
    [InlineData("/uzmanlik-alanlari", "Uzmanlık alanları")]
    [InlineData("/iletisim", "İletişim")]
    [InlineData("/makaleler/test-makalesi", "Yayımlanan test makalesi")]
    [InlineData("/uzmanlik-alanlari/test-alani", "Test çalışma alanı")]
    public async Task PublicRoutes_ReturnHtmlWithAccessiblePageStructure(string path, string heading)
    {
        using var client = factory.CreateHttpsClient();
        var response = await client.GetAsync(path);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", response.Content.Headers.ContentType?.CharSet);
        Assert.Contains(heading, WebUtility.HtmlDecode(html), StringComparison.Ordinal);
        Assert.Single(H1Tags().Matches(html));
        Assert.Contains("lang=\"tr\"", html, StringComparison.Ordinal);
        Assert.Contains("href=\"#icerik\"", html, StringComparison.Ordinal);
        Assert.Contains("id=\"icerik\"", html, StringComparison.Ordinal);
        Assert.Contains("name=\"description\"", html, StringComparison.Ordinal);
        Assert.Contains("property=\"og:title\"", html, StringComparison.Ordinal);
        Assert.Contains("https://cevatbiyik.av.tr" + path, html, StringComparison.Ordinal);
        Assert.DoesNotContain("TEST_ONLY_PRIVATE_VALUE", html, StringComparison.Ordinal);
        Assert.DoesNotContain("TEST_ONLY_INACTIVE_VALUE", html, StringComparison.Ordinal);
        Assert.True(response.Headers.Contains("Content-Security-Policy"));
        Assert.Contains("nosniff", response.Headers.GetValues("X-Content-Type-Options"));
    }

    [Theory]
    [InlineData("/makaleler/gizli-taslak")]
    [InlineData("/makaleler/gelecek-yayin")]
    [InlineData("/makaleler/silinen-yayin")]
    [InlineData("/makaleler/pasif-alan-yayini")]
    [InlineData("/makaleler/bulunmayan")]
    [InlineData("/uzmanlik-alanlari/gizli-alan")]
    [InlineData("/uzmanlik-alanlari/silinen-alan")]
    public async Task NonPublicDetails_Return404(string path)
    {
        using var client = factory.CreateHttpsClient();
        var response = await client.GetAsync(path);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("noindex", response.Headers.GetValues("X-Robots-Tag").Single(), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("/makaleler?page=0")]
    [InlineData("/makaleler?page=invalid")]
    [InlineData("/makaleler?category=-1")]
    [InlineData("/makaleler?categoryPage=0")]
    [InlineData("/uzmanlik-alanlari?page=0")]
    public async Task InvalidQuery_Returns400(string path)
    {
        using var client = factory.CreateHttpsClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync(path)).StatusCode);
    }

    [Fact]
    public async Task SearchAndCategory_ApplyBeforePaginationAndKeepFilters()
    {
        using var client = factory.CreateHttpsClient();
        var found = WebUtility.HtmlDecode(await client.GetStringAsync("/makaleler?q=özeti&category=1"));
        Assert.Contains("Yayımlanan test makalesi", found, StringComparison.Ordinal);
        Assert.Contains("1 makale", found, StringComparison.Ordinal);
        var missing = WebUtility.HtmlDecode(await client.GetStringAsync("/makaleler?q=eşleşmeyen"));
        Assert.Contains("Makale bulunamadı", missing, StringComparison.Ordinal);
        Assert.Contains("noindex", missing, StringComparison.Ordinal);
        Assert.DoesNotContain("Gizli taslak", found, StringComparison.Ordinal);
    }

    [Fact]
    public async Task LegacyArticleContent_IsEncodedInBodyAndMetaTags()
    {
        using var isolated = new PublicWebFactory();
        isolated.Articles.Seed(new CevatBiyik.Domain.Entities.Article
        {
            Id = 100,
            Slug = "encoding-test",
            Title = "<script>alert(1)</script>",
            Excerpt = "\" /><script>alert(2)</script>",
            Content = "<img src=x onerror=alert(3)>",
            IsActive = true,
            PublishedAt = DateTime.UtcNow.AddDays(-1)
        });
        using var client = isolated.CreateHttpsClient();
        var html = await client.GetStringAsync("/makaleler/encoding-test");
        Assert.DoesNotContain("<script>alert", html, StringComparison.Ordinal);
        Assert.DoesNotContain("<img src=x", html, StringComparison.Ordinal);
        Assert.Contains("&lt;img", html, StringComparison.Ordinal);
        Assert.Contains("&lt;script&gt;", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Canonical_UsesConfiguredHostAndIgnoresUntrustedHost()
    {
        using var client = factory.CreateHttpsClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/makaleler?page=2");
        request.Headers.Host = "attacker.invalid";
        var html = await (await client.SendAsync(request)).Content.ReadAsStringAsync();
        Assert.Contains("href=\"https://cevatbiyik.av.tr/makaleler?page=2\"", html, StringComparison.Ordinal);
        Assert.DoesNotContain("attacker.invalid", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("/lib/bootstrap/bootstrap.min.css", "text/css")]
    [InlineData("/css/site.css", "text/css")]
    [InlineData("/js/site.js", "text/javascript")]
    [InlineData("/images/brand/cevat-biyik-emblem.svg", "image/svg+xml")]
    public async Task LocalAssets_AreServed(string path, string contentType)
    {
        using var client = factory.CreateHttpsClient();
        var response = await client.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(contentType, response.Content.Headers.ContentType?.MediaType);
    }

    [GeneratedRegex("<h1(?:\\s|>)", RegexOptions.IgnoreCase)]
    private static partial Regex H1Tags();

    [Fact]
    public async Task SearchForm_PostRedirectGetDoesNotPutCsrfTokenInUrl()
    {
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/makaleler");
        var token = SearchTokenPattern().Match(html).Groups[1].Value;
        Assert.NotEmpty(token);
        var response = await client.PostAsync("/makaleler", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = WebUtility.HtmlDecode(token),
            ["q"] = "test",
            ["category"] = "1"
        }));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = response.Headers.Location!.OriginalString;
        Assert.Contains("q=test", location, StringComparison.Ordinal);
        Assert.Contains("category=1", location, StringComparison.Ordinal);
        Assert.DoesNotContain("RequestVerificationToken", location, StringComparison.Ordinal);
        Assert.Contains("test-makalesi", await client.GetStringAsync(location), StringComparison.Ordinal);
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex SearchTokenPattern();
}
