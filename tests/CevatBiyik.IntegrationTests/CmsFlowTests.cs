using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace CevatBiyik.IntegrationTests;
public sealed partial class CmsFlowTests
{
    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex TokenPattern();
    private static async Task<string> Token(HttpClient client, string path) => WebUtility.HtmlDecode(TokenPattern().Match(await client.GetStringAsync(path)).Groups[1].Value);
    private static async Task<HttpResponseMessage> Post(HttpClient client, string form, string path, Dictionary<string, string> values)
    { values["__RequestVerificationToken"] = await Token(client, form); return await client.PostAsync(path, new FormUrlEncodedContent(values)); }
    private static Task<HttpResponseMessage> Login(HttpClient client) => Post(client, "/admin/login", "/admin/login", new() { ["UserName"] = "admin", ["Password"] = AdminWebFactory.Password });
    [Fact]
    public async Task TaxonomyAndArticleSeo_WorkThroughRealAdminMvcPersistenceAndPublicRendering()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync(); using var client = factory.CreateHttpsClient();
        foreach (var path in new[] { "/admin/taxonomy/categories", "/admin/taxonomy/tags", "/admin/media", "/admin/page-seo" })
        {
            Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync(path)).StatusCode);
        }

        await Login(client);
        foreach (var kind in new[] { "categories", "tags" })
        {
            Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync($"/admin/taxonomy/{kind}/save", new FormUrlEncodedContent(new Dictionary<string, string>()))).StatusCode);
            Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/admin/taxonomy/{kind}/edit", $"/admin/taxonomy/{kind}/save", new() { ["Name"] = "Test " + kind, ["IsActive"] = "true" })).StatusCode);
        }
        var values = new Dictionary<string, string> { ["Title"] = "Test CMS makalesi", ["Content"] = "Test içerik", ["Excerpt"] = "Test özet", ["IsActive"] = "true", ["PublishedAt"] = "2020-01-01T12:00", ["CategoryId"] = "1", ["TagIds"] = "1", ["Seo.MetaTitle"] = "Özel meta başlık", ["Seo.MetaDescription"] = "Özel açıklama", ["Seo.Keywords"] = "test, cms", ["Seo.OpenGraphTitle"] = "Paylaşım başlığı" };
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/articles/create", "/admin/articles/save", values)).StatusCode);
        await CmsPreview.ExportAsync(factory, client, ("article-editor", "/admin/articles/edit/1"), ("categories", "/admin/taxonomy/categories"), ("page-seo", "/admin/page-seo/edit"), ("articles", "/makaleler"));
        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/makaleler/test-cms-makalesi"));
        Assert.Contains("<title>Özel meta başlık</title>", html, StringComparison.Ordinal);
        Assert.Contains("content=\"Paylaşım başlığı\"", html, StringComparison.Ordinal);
        Assert.Contains("Test tags", html, StringComparison.Ordinal);
        Assert.Contains("Test categories", html, StringComparison.Ordinal);
        Assert.Contains("\"@type\":\"Article\"", html, StringComparison.Ordinal);
        Assert.Contains("\"@type\":\"BreadcrumbList\"", html, StringComparison.Ordinal);
        Assert.Contains("Test CMS makalesi", WebUtility.HtmlDecode(await client.GetStringAsync("/makaleler?topic=1&tag=test-tags")), StringComparison.Ordinal);
        Assert.DoesNotContain("Test CMS makalesi", WebUtility.HtmlDecode(await client.GetStringAsync("/makaleler?topic=99")), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/articles/edit/1", "/admin/articles/save/1", values)).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.Single(await db.ArticleTagLinks.ToArrayAsync());
            Assert.Contains(await db.AuditLogs.ToArrayAsync(), x => x.Action == "TaxonomySaved");
        }
        values.Remove("TagIds"); values["PublishedAt"] = "2099-01-01T12:00";
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/articles/edit/1", "/admin/articles/save/1", values)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/makaleler/test-cms-makalesi")).StatusCode);
    }
    [Fact]
    public async Task MediaUpload_RequiresCsrf_ResizesImage_AndProtectsUsedAssets()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync(); using var client = factory.CreateHttpsClient(); await Login(client);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/admin/media/upload", new MultipartFormDataContent())).StatusCode);
        using var body = new MultipartFormDataContent(); body.Add(new StringContent(await Token(client, "/admin/media")), "__RequestVerificationToken");
        body.Add(new StringContent("Test görseli"), "altText"); using var content = new ByteArrayContent(ImageStorageTests.Png()); content.Headers.ContentType = new MediaTypeHeaderValue("image/png"); body.Add(content, "file", "test.png");
        Assert.Equal(HttpStatusCode.Redirect, (await client.PostAsync("/admin/media/upload", body)).StatusCode);
        await CmsPreview.ExportAsync(factory, client, ("media", "/admin/media"));
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var image = await db.MediaAssets.SingleAsync();
            Assert.Equal(1600, image.Width); Assert.Equal(800, image.Height); Assert.True(File.Exists(Path.Combine(factory.UploadRoot, image.StoragePath)));
            db.Articles.Add(new Article { Title = "Test", Slug = "test", Content = "Test", CoverMediaId = image.Id }); await db.SaveChangesAsync();
        }
        await Post(client, "/admin/media", "/admin/media/delete/1", new() { ["confirmed"] = "true" });
        Assert.Contains("içerikte kullanılıyor", WebUtility.HtmlDecode(await client.GetStringAsync("/admin/media")), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/uploads/attack.aspx")).StatusCode);
    }
    [Fact]
    public async Task FailedUploadAudit_RollsBackRowAndRemovesFilesystemAsset()
    {
        using var factory = new AdminWebFactory { FailMediaAudit = true }; await factory.InitializeAsync(); using var client = factory.CreateHttpsClient(); await Login(client);
        using var body = new MultipartFormDataContent(); body.Add(new StringContent(await Token(client, "/admin/media")), "__RequestVerificationToken");
        body.Add(new StringContent("Test görseli"), "altText"); using var content = new ByteArrayContent(ImageStorageTests.Png()); content.Headers.ContentType = new MediaTypeHeaderValue("image/png"); body.Add(content, "file", "test.png");
        Assert.Equal(HttpStatusCode.InternalServerError, (await client.PostAsync("/admin/media/upload", body)).StatusCode);
        using var scope = factory.Services.CreateScope();
        Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().MediaAssets.ToArrayAsync());
        Assert.Empty(Directory.GetFiles(Path.Combine(factory.UploadRoot, "images")));
    }
    [Fact]
    public async Task SitemapAndRobots_RespectVisibilityAndTrustedOrigin()
    {
        using var factory = new AdminWebFactory { AllowIndexing = true }; await factory.InitializeAsync(); using var client = factory.CreateHttpsClient();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Articles.AddRange(new Article { Title = "Test", Slug = "visible", Content = "Test", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1) },
                new Article { Slug = "future", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(1) }, new Article { Slug = "draft" }, new Article { Slug = "deleted", IsDeleted = true, IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1) });
            db.PracticeAreas.Add(new PracticeArea { Name = "Test", Slug = "area", Content = "Test", IsActive = true }); await db.SaveChangesAsync();
        }
        var response = await client.GetAsync("/sitemap.xml"); Assert.Equal("application/xml", response.Content.Headers.ContentType?.MediaType);
        var index = XDocument.Parse(await response.Content.ReadAsStringAsync()); Assert.Equal("sitemapindex", index.Root!.Name.LocalName);
        var xml = await client.GetStringAsync("/sitemaps/articles/1.xml");
        Assert.Contains("https://cevatbiyik.av.tr/makaleler/visible", xml, StringComparison.Ordinal);
        Assert.DoesNotContain("future", xml, StringComparison.Ordinal); Assert.DoesNotContain("draft", xml, StringComparison.Ordinal); Assert.DoesNotContain("deleted", xml, StringComparison.Ordinal);
        Assert.Contains("Disallow: /admin", await client.GetStringAsync("/robots.txt"), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/sitemaps/articles/2.xml")).StatusCode);
        using var disabled = new PublicWebFactory(); using var disabledClient = disabled.CreateHttpsClient();
        Assert.Contains("Disallow: /\n", await disabledClient.GetStringAsync("/robots.txt"), StringComparison.Ordinal);
        Assert.Empty(XDocument.Parse(await disabledClient.GetStringAsync("/sitemap.xml")).Root!.Elements());
    }
    [Fact]
    public async Task PageSeoAndJsonLd_AreEncodedAndLegalServiceRequiresVerifiedVisibleFacts()
    {
        using var factory = new AdminWebFactory { LegalServiceSchemaEnabled = true }; await factory.InitializeAsync(); using var client = factory.CreateHttpsClient(); await Login(client);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/page-seo/edit", "/admin/page-seo/save", new() { ["Path"] = "/", ["IsActive"] = "true", ["Seo.MetaTitle"] = "Ana sayfa özel başlık" })).StatusCode);
        Assert.Contains("<title>Ana sayfa özel başlık</title>", WebUtility.HtmlDecode(await client.GetStringAsync("/")), StringComparison.Ordinal);
        Assert.DoesNotContain("LegalService", await client.GetStringAsync("/iletisim"), StringComparison.Ordinal);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SiteSettings.AddRange(new SiteSetting { Key = "Contact.Email", Value = "test@example.test", IsActive = true }, new SiteSetting { Key = "Contact.Phone", Value = "+90 000 000 00 00", IsActive = true }, new SiteSetting { Key = "Contact.Address", Value = "Test adresi", IsActive = true });
            db.Articles.Add(new Article { Title = "</script><script>alert(1)</script>", Content = "Test", Slug = "encoding", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1) }); await db.SaveChangesAsync();
        }
        Assert.Contains("\"@type\":\"LegalService\"", await client.GetStringAsync("/iletisim"), StringComparison.Ordinal);
        var response = await client.GetAsync("/makaleler/encoding"); var html = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("</script><script>alert(1)</script>", html, StringComparison.Ordinal);
        Assert.Contains("\\u003C/script\\u003E", html, StringComparison.Ordinal);
        Assert.Contains("'nonce-", response.Headers.GetValues("Content-Security-Policy").Single(), StringComparison.Ordinal);
    }
}
