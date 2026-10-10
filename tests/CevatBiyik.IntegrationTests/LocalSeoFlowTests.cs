using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace CevatBiyik.IntegrationTests;
public sealed partial class LocalSeoFlowTests
{
    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex Token();
    [GeneratedRegex("<script[^>]*type=\"application/ld(?:\\+|&#x2B;)json\"[^>]*>(.*?)</script>", RegexOptions.Singleline)]
    private static partial Regex JsonLd();
    private static string Body(string prefix) => string.Join(' ', Enumerable.Range(0, 160).Select(i => prefix + i));
    private static string Details(string prefix) => string.Join(' ', Enumerable.Range(200, 45).Select(i => prefix + i));
    private static async Task<HttpResponseMessage> Post(HttpClient client, string form, string path, Dictionary<string, string> fields)
    {
        fields["__RequestVerificationToken"] = WebUtility.HtmlDecode(Token().Match(await client.GetStringAsync(form)).Groups[1].Value);
        return await client.PostAsync(path, new FormUrlEncodedContent(fields));
    }
    private static Dictionary<string, string> Fields(string name = "Kayseri", string prefix = "fixturea") => new()
    {
        ["Name"] = name, ["Kind"] = "0", ["Summary"] = "TEST_ONLY yerel özet", ["Body"] = Body(prefix), ["LocalDetails"] = Details(prefix),
        ["IsActive"] = "true", ["IsReviewed"] = "true", ["FactsVerified"] = "true", ["PracticeAreaIds"] = "1", ["ArticleIds"] = "1",
        ["Seo.MetaTitle"] = "TEST_ONLY yerel başlık", ["Seo.MetaDescription"] = "TEST_ONLY yerel açıklama", ["Seo.OpenGraphTitle"] = "TEST_ONLY yerel paylaşım"
    };
    private static async Task SeedAsync(AdminWebFactory factory)
    {
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.PracticeAreas.Add(new() { Name = "TEST_ONLY area", Slug = "local-area", Content = "TEST_ONLY", IsActive = true });
        db.Articles.Add(new() { Title = "TEST_ONLY article", Slug = "local-article", Content = "TEST_ONLY", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1) });
        await db.SaveChangesAsync();
    }
    private static async Task Login(HttpClient client, string user = "admin") => Assert.Equal(HttpStatusCode.Redirect,
        (await Post(client, "/admin/login", "/admin/login", new() { ["UserName"] = user, ["Password"] = AdminWebFactory.Password })).StatusCode);
    [Fact]
    public async Task SitemapIndexLimit_IncludesTheLocalChildSitemap()
    {
        using var factory = new PublicWebFactory();
        factory.Locations.Seed(new Location { Id = 1, Name = "Kayseri", Slug = "kayseri", Summary = "TEST_ONLY", Body = Body("fixturea"),
            LocalDetails = Details("fixturea"), IsActive = true, IsReviewed = true, FactsVerified = true,
            PracticeLinks = [new() { PracticeAreaId = 1, PracticeArea = factory.Areas.Items[0], IsActive = true }] });
        using var configured = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?> { ["Site:AllowIndexing"] = "true" }));
            builder.ConfigureServices(services => { services.RemoveAll<ISitemapService>(); services.AddSingleton<ISitemapService>(new BoundarySitemap()); });
        });
        using var client = configured.CreateClient(new() { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        Assert.Equal(HttpStatusCode.ServiceUnavailable, (await client.GetAsync("/sitemap.xml")).StatusCode);
    }
    private sealed class BoundarySitemap : ISitemapService
    {
        public Task<SitemapCounts> CountAsync(CancellationToken cancellationToken = default) => Task.FromResult(new SitemapCounts(49999000, 0));
        public Task<IReadOnlyList<SitemapEntry>> ListAsync(bool articles, int page, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<SitemapEntry>>([]);
    }
    [Theory]
    [InlineData(true)] [InlineData(false)]
    public async Task AdminCrudAndPublicLinks_RespectAuthCsrfIndexingAndSoftDeletion(bool indexing)
    {
        using var factory = new AdminWebFactory { AllowIndexing = indexing }; await factory.InitializeAsync(); await SeedAsync(factory);
        using var client = factory.CreateHttpsClient(); Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/admin/locations")).StatusCode);
        await Login(client); Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/admin/locations/save", new FormUrlEncodedContent(Fields()))).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/locations/edit", "/admin/locations/save", Fields())).StatusCode);
        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/konumlar/kayseri"));
        Assert.Contains("TEST_ONLY yerel başlık", html, StringComparison.Ordinal); Assert.Contains("TEST_ONLY yerel paylaşım", html, StringComparison.Ordinal);
        Assert.Contains("href=\"/uzmanlik-alanlari/local-area\"", html, StringComparison.Ordinal); Assert.Contains("href=\"/makaleler/local-article\"", html, StringComparison.Ordinal);
        Assert.Contains(indexing ? "index, follow" : "noindex, nofollow", html, StringComparison.Ordinal);
        using var schema = JsonDocument.Parse(JsonLd().Match(html).Groups[1].Value);
        Assert.Equal("BreadcrumbList", schema.RootElement[0].GetProperty("@type").GetString());
        Assert.Contains("href=\"/konumlar/kayseri\"", await client.GetStringAsync("/uzmanlik-alanlari/local-area"), StringComparison.Ordinal);
        Assert.Contains("href=\"/konumlar/kayseri\"", await client.GetStringAsync("/makaleler/local-article"), StringComparison.Ordinal);
        if (indexing)
        {
            Assert.Contains("/sitemaps/locations.xml", await client.GetStringAsync("/sitemap.xml"), StringComparison.Ordinal);
            Assert.Contains("/konumlar/kayseri", await client.GetStringAsync("/sitemaps/locations.xml"), StringComparison.Ordinal);
        }
        else
        {
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/sitemaps/locations.xml")).StatusCode);
        }
        await CmsPreview.ExportAsync(factory, client, ("local-index", "/konumlar"), ("local-detail", "/konumlar/kayseri"), ("location-editor", "/admin/locations/edit/1"), ("locations-admin", "/admin/locations"));
        var update = Fields(); update.Remove("ArticleIds");
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/locations/edit/1", "/admin/locations/save/1", update)).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); Assert.Empty(await db.LocationArticles.ToListAsync());
            Assert.Contains(await db.AuditLogs.ToListAsync(), x => x.Action == nameof(SecurityEvent.LocationSaved));
        }
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/admin/locations", "/admin/locations/delete/1", new())).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/locations", "/admin/locations/delete/1", new() { ["confirmed"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/konumlar/kayseri")).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); Assert.True((await db.Locations.IgnoreQueryFilters().SingleAsync()).IsDeleted);
        }
        using var reader = factory.CreateHttpsClient();
        Assert.Equal(HttpStatusCode.OK, (await Post(reader, "/admin/login", "/admin/login", new() { ["UserName"] = "reader", ["Password"] = AdminWebFactory.Password })).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await reader.GetAsync("/admin/locations")).StatusCode);
    }
    [Fact]
    public async Task ThinAndDuplicateContent_IsNoindexAndAbsentFromDiscoveryAtRuntime()
    {
        using var factory = new AdminWebFactory { AllowIndexing = true }; await factory.InitializeAsync(); await SeedAsync(factory);
        using var client = factory.CreateHttpsClient(); await Login(client);
        var thin = Fields(); thin["Body"] = "TEST_ONLY thin";
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/locations/edit", "/admin/locations/save", thin)).StatusCode);
        Assert.Contains("noindex, follow", await client.GetStringAsync("/konumlar/kayseri"), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/sitemaps/locations.xml")).StatusCode);
        Assert.DoesNotContain("href=\"/konumlar/kayseri\"", await client.GetStringAsync("/uzmanlik-alanlari/local-area"), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/locations/edit/1", "/admin/locations/save/1", Fields())).StatusCode);
        var district = Fields("Talas"); district["Kind"] = "1"; district["ParentId"] = "1";
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/locations/edit", "/admin/locations/save", district)).StatusCode);
        foreach (var path in new[] { "/konumlar/kayseri", "/konumlar/talas" })
        {
            Assert.Contains("noindex, follow", await client.GetStringAsync(path), StringComparison.Ordinal);
        }
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/sitemaps/locations.xml")).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); db.RedirectRules.Add(new() { SourcePath = "/konumlar/kayseri", DestinationPath = "/", IsActive = true }); await db.SaveChangesAsync();
        }
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/konumlar/kayseri")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/konumlar?page=invalid")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/konumlar/not-published")).StatusCode);
    }
    [Fact]
    public async Task InvalidHierarchyPlaceholderAndXss_DoNotPersist()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync(); await SeedAsync(factory); using var client = factory.CreateHttpsClient(); await Login(client);
        foreach (var invalid in new[] { "xss", "placeholder", "hierarchy" })
        {
            var fields = Fields(); fields[invalid == "xss" ? "Body" : invalid == "placeholder" ? "LocalDetails" : "Kind"] = invalid == "xss" ? "<script>TEST_ONLY</script>" : invalid == "placeholder" ? "CONTENT_REQUIRED" : "1";
            using var response = await Post(client, "/admin/locations/edit", "/admin/locations/save", fields);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.DoesNotContain("<script>TEST_ONLY</script>", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        }
        using var scope = factory.Services.CreateScope(); Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().Locations.ToListAsync());
    }
    [Theory]
    [InlineData(false, false)] [InlineData(true, false)] [InlineData(false, true)] [InlineData(true, true)]
    public async Task SchemaSignals_RequireExplicitFlagsReviewedProfileAndCompleteVisibleContacts(bool legal, bool person)
    {
        using var factory = new AdminWebFactory { AllowIndexing = true, LegalServiceSchemaEnabled = legal, PersonSchemaEnabled = person }; await factory.InitializeAsync(); await SeedAsync(factory);
        using var client = factory.CreateHttpsClient(); await Login(client); Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/locations/edit", "/admin/locations/save", Fields())).StatusCode);
        Assert.DoesNotContain("LegalService", await client.GetStringAsync("/konumlar/kayseri"), StringComparison.Ordinal);
        Assert.DoesNotContain("\"Person\"", await client.GetStringAsync("/hakkimda"), StringComparison.Ordinal);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SiteSettings.AddRange(new SiteSetting { Key = "Contact.Email", Value = "test@example.test", IsActive = true },
                new SiteSetting { Key = "Contact.Phone", Value = "+90 555 000 0000", IsActive = true }, new SiteSetting { Key = "Contact.Address", Value = "TEST_ONLY address", IsActive = true });
            db.LawyerProfiles.Add(new() { Biography = "TEST_ONLY biography", Vision = "TEST_ONLY vision", BarAssociation = "TEST_ONLY bar", IsActive = true, IsReviewed = true });
            await db.SaveChangesAsync();
        }
        var local = JsonDocument.Parse(JsonLd().Match(await client.GetStringAsync("/konumlar/kayseri")).Groups[1].Value);
        Assert.Equal(legal ? 2 : 1, local.RootElement.GetArrayLength());
        if (legal)
        {
            Assert.Equal("Kayseri", local.RootElement[1].GetProperty("areaServed").GetProperty("name").GetString());
            Assert.Equal("TEST_ONLY address", local.RootElement[1].GetProperty("address").GetString());
        }
        var about = await client.GetStringAsync("/hakkimda"); Assert.Equal(person, JsonLd().IsMatch(about));
        if (person)
        {
            using var data = JsonDocument.Parse(JsonLd().Match(about).Groups[1].Value); Assert.Equal("Person", data.RootElement.GetProperty("@type").GetString());
            Assert.Equal("Av. Cevat Bıyık", data.RootElement.GetProperty("name").GetString());
            Assert.DoesNotContain("hasCredential", about, StringComparison.Ordinal);
        }
    }
    [Fact]
    public async Task AuditFailure_RollsBackLocationsAndRelationshipRows()
    {
        using var factory = new AdminWebFactory { FailLocationAudit = true }; await factory.InitializeAsync(); await SeedAsync(factory);
        using var scope = factory.Services.CreateScope(); var operations = scope.ServiceProvider.GetRequiredService<IAdminOperation>(); var locations = scope.ServiceProvider.GetRequiredService<ILocationService>();
        await Assert.ThrowsAsync<InvalidOperationException>(() => operations.ExecuteAsync(SecurityEvent.LocationSaved, "test-only", () => locations.SaveAsync(null, new LocationInput { Name = "Kayseri", PracticeAreaIds = [1] })));
        scope.ServiceProvider.GetRequiredService<AppDbContext>().ChangeTracker.Clear();
        Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().Locations.ToListAsync());
        Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().LocationPracticeAreas.ToListAsync());
    }
}
