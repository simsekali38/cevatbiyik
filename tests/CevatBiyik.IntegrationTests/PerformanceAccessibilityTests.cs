using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CevatBiyik.IntegrationTests;

public sealed class PerformanceAccessibilityTests(PublicWebFactory factory) : IClassFixture<PublicWebFactory>
{
    [Theory]
    [InlineData("/css/site.min.css", "gzip")]
    [InlineData("/js/site.min.js", "br")]
    [InlineData("/lib/bootstrap/bootstrap.min.css", "br")]
    public async Task PublicAssets_AreCompressedWithoutLosingSecurityHeaders(string path, string encoding)
    {
        using var client = factory.CreateHttpsClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.AcceptEncoding.ParseAdd(encoding);
        using var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(encoding, response.Content.Headers.ContentEncoding);
        Assert.Contains("Accept-Encoding", response.Headers.Vary);
        Assert.Contains("nosniff", response.Headers.GetValues("X-Content-Type-Options"));
        var bytes = await response.Content.ReadAsByteArrayAsync();
        using var stream = new MemoryStream(bytes);
        using Stream decoder = encoding == "br" ? new BrotliStream(stream, CompressionMode.Decompress) : new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(decoder, Encoding.UTF8);
        Assert.NotEmpty(await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task AssetCache_ValidatesActualHash_AndSupportsConditionalRequests()
    {
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/");
        var url = WebUtility.HtmlDecode(Regex.Match(html, "href=\"([^\"]*site.min.css[^\"]*)\"").Groups[1].Value);
        Assert.Contains("?v=", url, StringComparison.Ordinal);
        using var fingerprint = await client.GetAsync(url);
        Assert.True(fingerprint.Headers.CacheControl?.Public);
        Assert.Equal(TimeSpan.FromDays(365), fingerprint.Headers.CacheControl?.MaxAge);
        Assert.Contains("immutable", fingerprint.Headers.CacheControl!.ToString(), StringComparison.Ordinal);
        using var forged = await client.GetAsync("/css/site.min.css?v=forged");
        Assert.Equal(TimeSpan.FromHours(1), forged.Headers.CacheControl?.MaxAge);
        Assert.DoesNotContain("immutable", forged.Headers.CacheControl!.ToString(), StringComparison.Ordinal);
        using var condition = new HttpRequestMessage(HttpMethod.Get, url);
        condition.Headers.IfNoneMatch.Add(fingerprint.Headers.ETag!);
        using var unchanged = await client.SendAsync(condition);
        Assert.Equal(HttpStatusCode.NotModified, unchanged.StatusCode);
        using var missing = await client.GetAsync("/css/missing.css?v=forged");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.True(missing.Headers.CacheControl?.NoStore);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/iletisim")]
    [InlineData("/admin/login")]
    public async Task Html_IsNeverCompressedOrSharedCached_AndNoncesRemainUnique(string path)
    {
        using var client = factory.CreateHttpsClient();
        client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, br");
        using var first = await client.GetAsync(path);
        using var second = await client.GetAsync(path);
        Assert.True(first.Headers.CacheControl?.NoStore);
        Assert.Empty(first.Content.Headers.ContentEncoding);
        var firstNonce = Regex.Match(string.Join(" ", first.Headers.GetValues("Content-Security-Policy")), "nonce-([^']+)").Groups[1].Value;
        var secondNonce = Regex.Match(string.Join(" ", second.Headers.GetValues("Content-Security-Policy")), "nonce-([^']+)").Groups[1].Value;
        Assert.NotEmpty(firstNonce); Assert.NotEqual(firstNonce, secondNonce);
    }

    [Fact]
    public async Task PublicImages_HaveDimensions_AndFooterIsLazy()
    {
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/");
        foreach (Match image in Regex.Matches(html, "<img\\b[^>]*>"))
        {
            Assert.Matches("width=\"[1-9][0-9]*\"", image.Value);
            Assert.Matches("height=\"[1-9][0-9]*\"", image.Value);
        }
        Assert.Contains("footer-brand\"><img loading=\"lazy\"", html, StringComparison.Ordinal);
        Assert.Contains("/css/site.min.css?v=", html, StringComparison.Ordinal);
        Assert.Contains("/js/site.min.js?v=", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ArticleCover_IsPrioritizedOnDetail_AndLazyOnList()
    {
        using var isolated = new PublicWebFactory();
        var article = isolated.Articles.Items.Single(x => x.Id == 1);
        article.CoverMediaId = 1;
        article.CoverMedia = new CevatBiyik.Domain.Entities.MediaAsset { Id = 1, StoragePath = "images/" + new string('a', 32) + ".webp",
            AltText = "TEST_ONLY cover", Width = 1200, Height = 800, IsActive = true, ContentType = "image/webp", Bytes = 1000 };
        using var client = isolated.CreateHttpsClient();
        var detail = await client.GetStringAsync("/makaleler/test-makalesi");
        var list = await client.GetStringAsync("/makaleler");
        Assert.Contains("loading=\"eager\" fetchpriority=\"high\"", detail, StringComparison.Ordinal);
        Assert.Contains("width=\"1200\" height=\"800\"", detail, StringComparison.Ordinal);
        Assert.Contains("loading=\"lazy\"", list, StringComparison.Ordinal);
        Assert.DoesNotContain("fetchpriority=\"high\"", list, StringComparison.Ordinal);
    }
}
