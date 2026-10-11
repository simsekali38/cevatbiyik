using System.Net;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.IntegrationTests;

public sealed class ArticleDesignTests
{
    [Fact]
    public async Task RelatedArticles_OnlyExposePublishedMatchingCmsRecords()
    {
        using var factory = new PublicWebFactory();
        var area = factory.Areas.Items.Single(x => x.Id == 1);
        factory.Articles.Seed(new Article
        {
            Id = 101, Title = "TEST_ONLY related", Slug = "test-related", Content = "<h2>TEST_ONLY heading</h2><p>TEST_ONLY body</p>",
            IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow,
            PracticeAreaId = 1, PracticeArea = area
        });
        using var client = factory.CreateHttpsClient();
        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/makaleler/test-makalesi"));
        Assert.Contains("class=\"article-related\"", html, StringComparison.Ordinal);
        Assert.Contains("/makaleler/test-related", html, StringComparison.Ordinal);
        Assert.DoesNotContain("/makaleler/gelecek-yayin", html, StringComparison.Ordinal);
        Assert.DoesNotContain("/makaleler/gizli-taslak", html, StringComparison.Ordinal);
        var detail = WebUtility.HtmlDecode(await client.GetStringAsync("/makaleler/test-related"));
        Assert.Contains("Güncelleme tarihi:", detail, StringComparison.Ordinal);
        Assert.Contains("<h2>TEST_ONLY heading</h2>", detail, StringComparison.Ordinal);
        Assert.Contains("class=\"article-contact\"", detail, StringComparison.Ordinal);
    }

    [Fact]
    public async Task PagingRetainsSearchAreaAndTaxonomyFilters_AndCanonical()
    {
        using var factory = new PublicWebFactory();
        var area = factory.Areas.Items.Single(x => x.Id == 1);
        var category = new ArticleCategory { Id = 10, Name = "TEST_ONLY category", Slug = "test-category", IsActive = true };
        var tag = new ArticleTag { Id = 10, Name = "TEST_ONLY tag", Slug = "test-tag", IsActive = true };
        factory.Categories.Seed(category);
        factory.Tags.Seed(tag);
        for (var i = 100; i < 113; i++)
        {
            var article = new Article
            {
                Id = i, Title = "TEST_ONLY paging " + i, Slug = "test-paging-" + i, Content = "TEST_ONLY body",
                Excerpt = "CONTENT_REQUIRED", IsActive = true, PublishedAt = DateTime.UtcNow.AddDays(-1),
                PracticeAreaId = 1, PracticeArea = area, CategoryId = 10, Category = category
            };
            article.TagLinks.Add(new ArticleTagLink { ArticleId = i, Article = article, TagId = 10, Tag = tag });
            factory.Articles.Seed(article);
        }
        using var client = factory.CreateHttpsClient();
        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/makaleler?q=TEST_ONLY&category=1&topic=10&tag=test-tag"));
        Assert.Contains("13 makale", html, StringComparison.Ordinal);
        Assert.Contains("page=2", html, StringComparison.Ordinal);
        Assert.Contains("topic=10", html, StringComparison.Ordinal);
        Assert.Contains("tag=test-tag", html, StringComparison.Ordinal);
        Assert.DoesNotContain("CONTENT_REQUIRED", html, StringComparison.Ordinal);
        var second = await client.GetStringAsync("/makaleler?q=TEST_ONLY&category=1&topic=10&tag=test-tag&page=2");
        Assert.Contains("https://cevatbiyik.av.tr/makaleler?page=2", second, StringComparison.Ordinal);
        Assert.Contains("noindex", second, StringComparison.Ordinal);
    }
}
