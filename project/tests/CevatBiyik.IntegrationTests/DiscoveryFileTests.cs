using System.Net;
using System.Text.Json;
using System.Xml.Linq;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;

public sealed class DiscoveryFileTests
{
    [Fact]
    public async Task DisabledIndexing_KeepsSitemapAndAiFilesEmptyWithoutChangingRobots()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using var client = factory.CreateHttpsClient();
        var response = await client.GetAsync("/sitemap.xml");
        Assert.Equal("application/xml", response.Content.Headers.ContentType!.MediaType);
        var xml = XDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("utf-8", xml.Declaration!.Encoding);
        Assert.Empty(xml.Root!.Elements());
        Assert.Contains("Disallow: /", await client.GetStringAsync("/robots.txt"), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/sitemaps/pages.xml")).StatusCode);
        using var json = JsonDocument.Parse(await client.GetStringAsync("/ai-catalog.json"));
        Assert.False(json.RootElement.GetProperty("enabled").GetBoolean());
        Assert.Empty(json.RootElement.GetProperty("resources").EnumerateArray());
        var llms = await client.GetStringAsync("/llms.txt");
        Assert.StartsWith("# Av.", llms, StringComparison.Ordinal);
        Assert.DoesNotContain("/makaleler/", llms, StringComparison.Ordinal);
    }

    [Fact]
    public async Task EnabledDiscovery_ReusesPublicationRules_AndSitemapChildrenResolve()
    {
        using var factory = new AdminWebFactory { AllowIndexing = true };
        await factory.InitializeAsync();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var active = new PracticeArea { Name = "TEST_ONLY alan", Slug = "approved-area", Content = "TEST_ONLY content", IsActive = true };
            var hidden = new PracticeArea { Name = "TEST_ONLY gizli", Slug = "hidden-area", Content = "TEST_ONLY", IsActive = false };
            db.PracticeAreas.AddRange(active, hidden);
            db.Articles.AddRange(
                new Article { Title = "TEST_ONLY [yayın]", Slug = "approved-article", Content = "TEST_ONLY content", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1), PracticeArea = active },
                new Article { Title = "TEST_ONLY gelecekte", Slug = "scheduled-article", Content = "TEST_ONLY", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(1) },
                new Article { Title = "TEST_ONLY taslak", Slug = "draft-article", Content = "TEST_ONLY", IsActive = false, PublishedAt = DateTime.UtcNow.AddDays(-1) },
                new Article { Title = "TEST_ONLY silinen", Slug = "deleted-article", Content = "TEST_ONLY", IsActive = true, IsDeleted = true, PublishedAt = DateTime.UtcNow.AddDays(-1) },
                new Article { Title = "TEST_ONLY pasif alan", Slug = "hidden-area-article", Content = "TEST_ONLY", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1), PracticeArea = hidden });
            db.PageContents.AddRange(
                new PageContent { Path = "/kvkk", Title = "TEST_ONLY KVKK", Body = "TEST_ONLY reviewed text", IsReviewed = true, IsActive = true },
                new PageContent { Path = "/yasal-uyari", Title = "TEST_ONLY unpublished", Body = "TEST_ONLY", IsReviewed = false, IsActive = true },
                new PageContent { Path = "/admin/injected", Title = "TEST_ONLY invalid path", Body = "TEST_ONLY", IsReviewed = true, IsActive = true });
            db.SiteSettings.Add(new SiteSetting { Key = "Smtp.Password", Value = "TEST_ONLY_PRIVATE_VALUE", IsActive = true });
            await db.SaveChangesAsync();
        }
        using var client = factory.CreateHttpsClient();
        var index = XDocument.Parse(await client.GetStringAsync("/sitemap.xml"));
        Assert.Equal("sitemapindex", index.Root!.Name.LocalName);
        Assert.Equal("http://www.sitemaps.org/schemas/sitemap/0.9", index.Root.Name.NamespaceName);
        foreach (var child in index.Descendants().Where(x => x.Name.LocalName == "loc"))
        {
            Assert.StartsWith("https://cevatbiyik.av.tr/", child.Value, StringComparison.Ordinal);
            var response = await client.GetAsync(new Uri(child.Value).PathAndQuery);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(XDocument.Parse(await response.Content.ReadAsStringAsync()).Declaration);
        }
        var pages = await client.GetStringAsync("/sitemaps/pages.xml");
        Assert.Contains("/kvkk", pages, StringComparison.Ordinal);
        Assert.DoesNotContain("/admin/injected", pages, StringComparison.Ordinal);
        using var json = JsonDocument.Parse(await client.GetStringAsync("/ai-catalog.json"));
        var urls = json.RootElement.GetProperty("resources").EnumerateArray().Select(x => x.GetProperty("url").GetString()).ToArray();
        Assert.Contains("https://cevatbiyik.av.tr/makaleler/approved-article", urls);
        Assert.Contains("https://cevatbiyik.av.tr/uzmanlik-alanlari/approved-area", urls);
        Assert.DoesNotContain(urls, x => x!.Contains("hidden", StringComparison.Ordinal) || x.Contains("scheduled", StringComparison.Ordinal) || x.Contains("draft", StringComparison.Ordinal) || x.Contains("deleted", StringComparison.Ordinal) || x.Contains("/admin", StringComparison.Ordinal));
        var llms = await client.GetStringAsync("/llms.txt");
        Assert.Contains("[TEST_ONLY \\[yayın\\]]", llms, StringComparison.Ordinal);
        Assert.Contains("/makaleler/approved-article", llms, StringComparison.Ordinal);
        Assert.DoesNotContain("TEST_ONLY_PRIVATE_VALUE", llms, StringComparison.Ordinal);
        Assert.DoesNotContain("scheduled-article", llms, StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/ai-catalog.json?page=invalid")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/ai-catalog.json?page=2")).StatusCode);
    }

    [Fact]
    public async Task SitemapAndAiCatalog_UseBoundedChunksForMoreThanOneThousandArticles()
    {
        using var factory = new AdminWebFactory { AllowIndexing = true };
        await factory.InitializeAsync();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Articles.AddRange(Enumerable.Range(1, 1001).Select(i => new Article { Title = "TEST_ONLY " + i, Slug = "pagination-" + i, Content = "TEST_ONLY", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1) }));
            await db.SaveChangesAsync();
        }
        using var client = factory.CreateHttpsClient();
        Assert.Contains("/sitemaps/articles/2.xml", await client.GetStringAsync("/sitemap.xml"), StringComparison.Ordinal);
        var second = XDocument.Parse(await client.GetStringAsync("/sitemaps/articles/2.xml"));
        Assert.Single(second.Root!.Elements());
        using var first = JsonDocument.Parse(await client.GetStringAsync("/ai-catalog.json"));
        Assert.Equal("https://cevatbiyik.av.tr/ai-catalog.json?page=2", first.RootElement.GetProperty("next").GetString());
        using var next = JsonDocument.Parse(await client.GetStringAsync("/ai-catalog.json?page=2"));
        Assert.Single(next.RootElement.GetProperty("resources").EnumerateArray());
        Assert.Equal(JsonValueKind.Null, next.RootElement.GetProperty("next").ValueKind);
    }

    [Fact]
    public async Task Favicon_IsLinkedFromPublicAndAdmin_AndLocalFilesAreServed()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        foreach (var route in new[] { "/", "/admin/login" })
        {
            var html = await client.GetStringAsync(route);
            Assert.Contains("rel=\"icon\"", html, StringComparison.Ordinal);
            Assert.Contains("/favicon.ico?v=", html, StringComparison.Ordinal);
            Assert.Contains("apple-touch-icon", html, StringComparison.Ordinal);
        }
        foreach (var path in new[] { "/favicon.ico", "/images/brand/favicon-32x32.png", "/images/brand/apple-touch-icon.png" })
        {
            var response = await client.GetAsync(path);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True((await response.Content.ReadAsByteArrayAsync()).Length > 100);
        }
    }
}
