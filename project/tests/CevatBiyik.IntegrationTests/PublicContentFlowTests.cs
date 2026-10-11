using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace CevatBiyik.IntegrationTests;
public sealed partial class PublicContentFlowTests
{
    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex TokenPattern();
    [GeneratedRegex("<script[^>]*type=\"application/ld(?:\\+|&#x2B;)json\"[^>]*>(.*?)</script>", RegexOptions.Singleline)]
    private static partial Regex JsonPattern();
    [GeneratedRegex("<h1(?:\\s[^>]*)?>", RegexOptions.IgnoreCase)]
    private static partial Regex Headings();
    private static async Task<HttpResponseMessage> Post(HttpClient client, string form, string path, Dictionary<string, string> values)
    {
        values["__RequestVerificationToken"] = WebUtility.HtmlDecode(TokenPattern().Match(await client.GetStringAsync(form)).Groups[1].Value);
        return await client.PostAsync(path, new FormUrlEncodedContent(values));
    }
    private static async Task Login(HttpClient client) => Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/login", "/admin/login", new() { ["UserName"] = "admin", ["Password"] = AdminWebFactory.Password })).StatusCode);
    [Theory]
    [InlineData("/")]
    [InlineData("/hakkimda")]
    [InlineData("/makaleler")]
    [InlineData("/uzmanlik-alanlari")]
    [InlineData("/iletisim")]
    [InlineData("/sss")]
    [InlineData("/kvkk")]
    [InlineData("/cerez-politikasi")]
    [InlineData("/yasal-uyari")]
    public async Task PublicRoutes_HaveSingleHeadingLocalLogoAndAccessibleLandmarks(string path)
    {
        using var factory = new PublicWebFactory(); using var client = factory.CreateHttpsClient();
        using var response = await client.GetAsync(path); var html = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); Assert.Equal("text/html", response.Content.Headers.ContentType!.MediaType);
        Assert.Single(Headings().Matches(html)); Assert.Contains("id=\"icerik\" tabindex=\"-1\"", html, StringComparison.Ordinal);
        Assert.Contains("cevat-biyik-emblem.svg", html, StringComparison.Ordinal); Assert.Contains("aria-label=\"Hukuki bilgiler\"", html, StringComparison.Ordinal);
        Assert.DoesNotContain("20261008_105655", html, StringComparison.Ordinal); Assert.DoesNotContain("20261008_105703", html, StringComparison.Ordinal);
        if (path is "/sss" or "/kvkk" or "/cerez-politikasi" or "/yasal-uyari" or "/hakkimda")
        {
            Assert.Contains("noindex", html, StringComparison.Ordinal);
        }
    }
    [Theory]
    [InlineData("/sss?page=0")]
    [InlineData("/sss?page=invalid")]
    [InlineData("/sss?page=100001")]
    public async Task InvalidFaqPages_Return400(string path)
    {
        using var factory = new PublicWebFactory(); using var client = factory.CreateHttpsClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync(path)).StatusCode);
    }
    [Fact]
    public async Task AdminCms_PublishesReviewedContent_AndSupportsDisableSoftDeleteAndSafeSchema()
    {
        using var factory = new AdminWebFactory { AllowIndexing = true }; await factory.InitializeAsync(); using var client = factory.CreateHttpsClient();
        foreach (var path in new[] { "/admin/profile", "/admin/page-contents", "/admin/faq" })
        {
            Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync(path)).StatusCode);
        }

        await Login(client);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/admin/profile/save", new FormUrlEncodedContent(new Dictionary<string, string>()))).StatusCode);
        var profile = new Dictionary<string, string> { ["Biography"] = "TEST_ONLY profil bilgisi", ["Vision"] = "TEST_ONLY vizyon", ["BarAssociation"] = "TEST_ONLY baro", ["IsActive"] = "true" };
        Assert.Equal(HttpStatusCode.OK, (await Post(client, "/admin/profile", "/admin/profile/save", profile)).StatusCode);
        Assert.DoesNotContain("TEST_ONLY profil bilgisi", await client.GetStringAsync("/hakkimda"), StringComparison.Ordinal);
        profile["IsReviewed"] = "true";
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/profile", "/admin/profile/save", profile)).StatusCode);
        Assert.Contains("TEST_ONLY profil bilgisi", await client.GetStringAsync("/hakkimda"), StringComparison.Ordinal);
        var reviewInput = Regex.Match(await client.GetStringAsync("/admin/profile"), "<input[^>]*id=\"IsReviewed\"[^>]*>").Value;
        Assert.NotEmpty(reviewInput);
        Assert.DoesNotContain("checked", reviewInput, StringComparison.Ordinal);
        foreach (var path in new[] { "/", "/kvkk", "/cerez-politikasi", "/yasal-uyari" })
        {
            Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/page-contents/edit?path=" + Uri.EscapeDataString(path), "/admin/page-contents/save", new()
            { ["Path"] = path, ["Title"] = "TEST_ONLY sayfa " + path, ["Body"] = "TEST_ONLY onaylı test metni", ["IsActive"] = "true", ["IsReviewed"] = "true" })).StatusCode);
        }
        for (var i = 1; i <= 13; i++)
        {
            Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/faq/edit", "/admin/faq/save", new()
            { ["Question"] = "TEST_ONLY soru " + i, ["Answer"] = "TEST_ONLY yanıt " + i, ["SortOrder"] = i.ToString(System.Globalization.CultureInfo.InvariantCulture), ["IsActive"] = "true", ["IsReviewed"] = "true" })).StatusCode);
        }
        var html = await client.GetStringAsync("/sss");
        using var json = JsonDocument.Parse(JsonPattern().Match(html).Groups[1].Value);
        Assert.Equal("FAQPage", json.RootElement.GetProperty("@type").GetString()); Assert.Equal(12, json.RootElement.GetProperty("mainEntity").GetArrayLength());
        Assert.Equal(12, Regex.Matches(html, "<details ").Count); Assert.Contains("faq-question-1", html, StringComparison.Ordinal);
        var second = await client.GetStringAsync("/sss?page=2"); Assert.Contains("TEST_ONLY soru 13", second, StringComparison.Ordinal); Assert.Contains("noindex", second, StringComparison.Ordinal);
        var sitemap = XDocument.Parse(await client.GetStringAsync("/sitemaps/pages.xml")); var urls = sitemap.Descendants().Where(x => x.Name.LocalName == "loc").Select(x => x.Value).ToArray();
        foreach (var path in new[] { "/hakkimda", "/sss", "/kvkk", "/cerez-politikasi", "/yasal-uyari" })
        {
            Assert.Contains("https://cevatbiyik.av.tr" + path, urls);
        }

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.PracticeAreas.Add(new PracticeArea { Name = "TEST_ONLY çalışma alanı", Slug = "test-public-area", Content = "TEST_ONLY alan açıklaması", IsActive = true });
            db.Articles.Add(new Article { Title = "TEST_ONLY makale", Slug = "test-public-article", Excerpt = "TEST_ONLY özet", Content = "TEST_ONLY makale içeriği", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1) });
            await db.SaveChangesAsync();
            Assert.Contains(await db.AuditLogs.ToArrayAsync(), x => x.Action == nameof(SecurityEvent.ProfileSaved));
        }
        await CmsPreview.ExportAsync(factory, client, ("public-home", "/"), ("public-profile", "/hakkimda"), ("public-faq", "/sss"), ("public-privacy", "/kvkk"), ("public-cookies", "/cerez-politikasi"), ("public-disclaimer", "/yasal-uyari"), ("public-contact", "/iletisim"), ("public-articles", "/makaleler"), ("public-article", "/makaleler/test-public-article"), ("public-areas", "/uzmanlik-alanlari"), ("public-area", "/uzmanlik-alanlari/test-public-area"), ("profile-editor", "/admin/profile"), ("faq-editor", "/admin/faq/edit/1"), ("page-editor", "/admin/page-contents/edit/2"));
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/page-contents", "/admin/page-contents/disable/2", new() { ["confirmed"] = "true" })).StatusCode);
        Assert.Contains("CONTENT_REQUIRED", await client.GetStringAsync("/kvkk"), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/faq", "/admin/faq/delete/1", new() { ["confirmed"] = "true" })).StatusCode);
        using var finalScope = factory.Services.CreateScope();
        Assert.True((await finalScope.ServiceProvider.GetRequiredService<AppDbContext>().FaqItems.IgnoreQueryFilters().SingleAsync(x => x.Id == 1)).IsDeleted);
    }
    [Fact]
    public async Task LegacyFaqMarkup_IsSanitized_WithPlainSchemaAnswer()
    {
        using var factory = new PublicWebFactory();
        factory.Faq.Seed(new FaqItem { Id = 1, Question = "Test sorusu", Answer = "<p><strong>Test yanıtı</strong></p></script><img src=x onerror=alert(2)>", IsActive = true, IsReviewed = true });
        factory.Faq.Seed(new FaqItem { Id = 2, Question = "<script>alert(1)</script>", Answer = "Test", IsActive = true, IsReviewed = true });
        using var client = factory.CreateHttpsClient(); var html = await client.GetStringAsync("/sss");
        Assert.DoesNotContain("<script>alert", html, StringComparison.Ordinal);
        Assert.DoesNotContain("<img src=x", html, StringComparison.Ordinal);
        Assert.DoesNotContain("onerror=alert", html, StringComparison.Ordinal);
        using var json = JsonDocument.Parse(JsonPattern().Match(html).Groups[1].Value);
        Assert.Equal(1, json.RootElement.GetProperty("mainEntity").GetArrayLength());
        Assert.Equal("Test yanıtı", json.RootElement.GetProperty("mainEntity")[0].GetProperty("acceptedAnswer").GetProperty("text").GetString());
    }
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ProductionContact_RequiresReviewedPublishedPrivacyPage(bool ready)
    {
        using var factory = new PublicWebFactory();
        if (ready)
        {
            factory.Pages.Seed(new PageContent { Id = 1, Path = "/kvkk", Title = "TEST_ONLY", Body = "TEST_ONLY notice", IsActive = true, IsReviewed = true });
        }

        using var production = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?> { ["Site:ContactFormEnabled"] = "true", ["Site:ContactNotice"] = "TEST_ONLY approved notice" }));
        });
        using var client = production.CreateClient(new() { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        var html = await client.GetStringAsync("/iletisim");
        if (!ready)
        {
            Assert.Contains("disabled=\"disabled\"", html, StringComparison.Ordinal);
        }
        else
        {
            Assert.DoesNotContain("disabled=\"disabled\"", html, StringComparison.Ordinal);
        }

        var response = await Post(client, "/iletisim", "/iletisim", new() { ["Form.Name"] = "TEST_ONLY", ["Form.Email"] = "test@example.test", ["Form.Message"] = "TEST_ONLY message", ["Form.Consent"] = "true" });
        Assert.Equal(ready ? HttpStatusCode.Redirect : HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
}
