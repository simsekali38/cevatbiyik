using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace CevatBiyik.IntegrationTests;

public sealed partial class SeoEnhancementTests
{
    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex Token();
    [GeneratedRegex("<script[^>]*type=\"application/ld(?:\\+|&#x2B;)json\"[^>]*>(.*?)</script>", RegexOptions.Singleline)]
    private static partial Regex JsonLd();
    private static async Task<HttpResponseMessage> Post(HttpClient client, string form, string path, Dictionary<string, string> fields)
    {
        fields["__RequestVerificationToken"] = WebUtility.HtmlDecode(Token().Match(await client.GetStringAsync(form)).Groups[1].Value);
        return await client.PostAsync(path, new FormUrlEncodedContent(fields));
    }
    [Theory]
    [InlineData("/")]
    [InlineData("/sss")]
    [InlineData("/hakkimda")]
    [InlineData("/kvkk")]
    [InlineData("/makaleler?q=test")]
    [InlineData("/makaleler/test-makalesi")]
    public async Task GlobalIndexingOff_WinsOverPagePolicyInMetaAndHeaders(string path)
    {
        using var factory = new PublicWebFactory(); using var client = factory.CreateHttpsClient();
        using var response = await client.GetAsync(path); var html = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("name=\"robots\" content=\"noindex, nofollow\"", html, StringComparison.Ordinal);
        Assert.Equal("noindex, nofollow", response.Headers.GetValues("X-Robots-Tag").Single());
        Assert.Single(Regex.Matches(html, "rel=\"canonical\""));
    }
    [Theory]
    [InlineData("https://evil.test/path")]
    [InlineData("//evil.test")]
    [InlineData("/admin/login")]
    [InlineData("/makaleler/gelecek-yayin")]
    [InlineData("/uzmanlik-alanlari/gizli-alan")]
    [InlineData("/kvkk")]
    public async Task StoredUnsafeOrUnpublishedRedirectTargets_DoNotRedirect(string target)
    {
        using var factory = new PublicWebFactory(); factory.Redirects.Seed(new RedirectRule { Id = 1, SourcePath = "/old", DestinationPath = target, IsActive = true });
        using var client = factory.CreateHttpsClient(); var response = await client.GetAsync("/old");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); Assert.Null(response.Headers.Location);
    }
    [Theory]
    [InlineData("GET")]
    [InlineData("HEAD")]
    public async Task Redirects_Return301WithoutQueryLeak_AndCurrentRoutesWin(string method)
    {
        using var factory = new PublicWebFactory();
        factory.Redirects.Seed(new RedirectRule { Id = 1, SourcePath = "/old", DestinationPath = "/makaleler/test-makalesi", IsActive = true });
        factory.Redirects.Seed(new RedirectRule { Id = 2, SourcePath = "/makaleler/test-makalesi", DestinationPath = "/", IsActive = true });
        using var client = factory.CreateHttpsClient(); var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(method), "/OLD?private=TEST_ONLY"));
        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode); Assert.Equal("/makaleler/test-makalesi", response.Headers.Location!.OriginalString);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/makaleler/test-makalesi")).StatusCode);
        Assert.NotEqual(HttpStatusCode.MovedPermanently, (await client.PostAsync("/old", new StringContent("TEST_ONLY"))).StatusCode);
    }
    [Theory]
    [InlineData("https://evil.test/path")]
    [InlineData("https://example.test/?query=1")]
    [InlineData("http://example.test")]
    [InlineData("https://user:password@example.test")]
    public async Task InvalidCanonicalOrigins_UseSafeConfiguredFallback(string origin)
    {
        using var factory = new PublicWebFactory(); using var configured = factory.WithWebHostBuilder(builder => builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?> { ["Site:BaseUrl"] = origin })));
        using var client = configured.CreateClient(new() { BaseAddress = new Uri("https://untrusted-request-host.test"), AllowAutoRedirect = false });
        var html = await client.GetStringAsync("/makaleler/test-makalesi");
        Assert.Contains("href=\"https://cevatbiyik.av.tr/makaleler/test-makalesi\"", html, StringComparison.Ordinal);
        Assert.DoesNotContain("https://untrusted-request-host.test", html, StringComparison.Ordinal);
    }
    [Fact]
    public async Task AdminRedirectAndAreaSeo_AreAuthorizedAuditedAndEditable()
    {
        using var factory = new AdminWebFactory { AllowIndexing = true }; await factory.InitializeAsync(); using var client = factory.CreateHttpsClient();
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/admin/redirects")).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/admin/seo")).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/login", "/admin/login", new() { ["UserName"] = "admin", ["Password"] = AdminWebFactory.Password })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/admin/redirects/save", new FormUrlEncodedContent(new Dictionary<string, string>()))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/admin/redirects?page=invalid")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await Post(client, "/admin/redirects/edit", "/admin/redirects/save", new() { ["SourcePath"] = "/old", ["DestinationPath"] = "//evil.test", ["IsActive"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/practice-areas/create", "/admin/practice-areas/save", new()
        { ["Name"] = "TEST_ONLY area", ["Content"] = "TEST_ONLY body", ["ShortDescription"] = "TEST_ONLY area summary", ["IsActive"] = "true",
            ["Seo.MetaTitle"] = "TEST_ONLY doğrulanmış uzmanlık alanı başlığı", ["Seo.MetaDescription"] = new string('A', 100),
            ["Seo.OpenGraphTitle"] = "TEST_ONLY paylaşım başlığı" })).StatusCode);
        string slug; long id;
        using (var scope = factory.Services.CreateScope())
        {
            var area = await scope.ServiceProvider.GetRequiredService<AppDbContext>().PracticeAreas.SingleAsync(); slug = area.Slug; id = area.Id;
        }
        var target = "/uzmanlik-alanlari/" + slug;
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/redirects/edit", "/admin/redirects/save", new() { ["SourcePath"] = "/old", ["DestinationPath"] = target, ["IsActive"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.MovedPermanently, (await client.GetAsync("/old")).StatusCode);
        var html = await client.GetStringAsync(target);
        Assert.Contains("TEST_ONLY paylaşım başlığı", WebUtility.HtmlDecode(html), StringComparison.Ordinal);
        using var schema = JsonDocument.Parse(JsonLd().Match(html).Groups[1].Value);
        Assert.Equal("BreadcrumbList", schema.RootElement.GetProperty("@type").GetString());
        Assert.Equal("https://cevatbiyik.av.tr" + target, schema.RootElement.GetProperty("itemListElement")[2].GetProperty("item").GetString());
        var panel = await client.GetStringAsync("/admin/seo"); Assert.Contains("SEO kontrolü", WebUtility.HtmlDecode(panel), StringComparison.Ordinal);
        Assert.Contains("İndeksleme", WebUtility.HtmlDecode(panel), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/admin/redirects", "/admin/redirects/disable/1", new())).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/redirects", "/admin/redirects/disable/1", new() { ["confirmed"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/old")).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.Equal(2, await db.AuditLogs.CountAsync(x => x.Action == nameof(SecurityEvent.RedirectSaved) || x.Action == nameof(SecurityEvent.RedirectDisabled)));
        }
        await CmsPreview.ExportAsync(factory, client, ("seo-panel", "/admin/seo"), ("redirect-editor", "/admin/redirects/edit/1"), ("area-seo-editor", "/admin/practice-areas/edit/" + id));
    }
    [Fact]
    public async Task SitemapAndSchema_OnlyExposePublishedCanonicalContent()
    {
        using var factory = new AdminWebFactory { AllowIndexing = true }; await factory.InitializeAsync();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Articles.AddRange(new Article { Title = "TEST_ONLY published", Slug = "published", Content = "TEST_ONLY", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1) },
                new Article { Title = "TEST_ONLY scheduled", Slug = "scheduled", Content = "TEST_ONLY", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(1) });
            db.RedirectRules.Add(new RedirectRule { SourcePath = "/old-published", DestinationPath = "/makaleler/published", IsActive = true }); await db.SaveChangesAsync();
        }
        using var client = factory.CreateHttpsClient();
        var robots = await client.GetStringAsync("/robots.txt"); Assert.Contains("Disallow: /admin", robots, StringComparison.Ordinal); Assert.Contains("https://cevatbiyik.av.tr/sitemap.xml", robots, StringComparison.Ordinal);
        var xml = XDocument.Parse(await client.GetStringAsync("/sitemaps/articles/1.xml")); var urls = xml.Descendants().Where(x => x.Name.LocalName == "loc").Select(x => x.Value).ToArray();
        Assert.Single(urls); Assert.Equal("https://cevatbiyik.av.tr/makaleler/published", urls[0]);
        var pages = await client.GetStringAsync("/sitemaps/pages.xml"); Assert.DoesNotContain("/kvkk", pages, StringComparison.Ordinal); Assert.DoesNotContain("/hakkimda", pages, StringComparison.Ordinal);
        Assert.DoesNotContain("/iletisim", pages, StringComparison.Ordinal);
        Assert.Contains("name=\"robots\" content=\"noindex, follow\"", await client.GetStringAsync("/iletisim"), StringComparison.Ordinal);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SiteSettings.AddRange(new SiteSetting { Key = "Contact.Email", Value = "test@example.test", IsActive = true },
                new SiteSetting { Key = "Contact.Phone", Value = "+90 000 000 00 00", IsActive = true },
                new SiteSetting { Key = "Contact.Address", Value = "TEST_ONLY address", IsActive = true }); await db.SaveChangesAsync();
        }
        Assert.Contains("/iletisim", await client.GetStringAsync("/sitemaps/pages.xml"), StringComparison.Ordinal);
        Assert.Contains("name=\"robots\" content=\"index, follow\"", await client.GetStringAsync("/iletisim"), StringComparison.Ordinal);
        var html = await client.GetStringAsync("/makaleler/published"); using var json = JsonDocument.Parse(JsonLd().Match(html).Groups[1].Value);
        Assert.Equal("Article", json.RootElement[0].GetProperty("@type").GetString()); Assert.Equal("TEST_ONLY published", json.RootElement[0].GetProperty("headline").GetString());
        Assert.Equal("https://cevatbiyik.av.tr/makaleler/published", json.RootElement[0].GetProperty("mainEntityOfPage").GetString());
        Assert.Contains("name=\"robots\" content=\"noindex, follow\"", await client.GetStringAsync("/makaleler?q=test"), StringComparison.Ordinal);
        Assert.Equal("noindex, nofollow", (await client.GetAsync("/unknown-page")).Headers.GetValues("X-Robots-Tag").Single());
    }
    [Fact]
    public async Task RedirectAuditFailure_RollsBackRule()
    {
        using var factory = new AdminWebFactory { FailRedirectAudit = true }; await factory.InitializeAsync(); using var client = factory.CreateHttpsClient();
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/login", "/admin/login", new() { ["UserName"] = "admin", ["Password"] = AdminWebFactory.Password })).StatusCode);
        Assert.Equal(HttpStatusCode.InternalServerError, (await Post(client, "/admin/redirects/edit", "/admin/redirects/save", new()
        { ["SourcePath"] = "/test-old", ["DestinationPath"] = "/", ["IsActive"] = "true" })).StatusCode);
        using var scope = factory.Services.CreateScope(); Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().RedirectRules.ToArrayAsync());
    }
}
