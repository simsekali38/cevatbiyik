using System.Net;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.IntegrationTests;

public sealed class RemainingSeoTests
{
    [Fact]
    public async Task LegacyPlaceholderMetadata_DoesNotOverrideValidPageDefaults()
    {
        using var factory = new PublicWebFactory();
        var area = factory.Areas.Items.Single(x => x.Id == 1);
        area.ShortDescription = "CONTENT_REQUIRED";
        area.MetaTitle = "CONTENT_REQUIRED";
        area.MetaDescription = "CONTENT_REQUIRED";
        area.Keywords = "CONTENT_REQUIRED";
        area.OpenGraphTitle = "CONTENT_REQUIRED";
        area.OpenGraphDescription = "CONTENT_REQUIRED";
        factory.Settings.Seed(new SiteSetting { Id = 10, Key = "Site.Name", Value = "CONTENT_REQUIRED", IsActive = true });
        factory.Settings.Seed(new SiteSetting { Id = 11, Key = "Site.Description", Value = "CONTENT_REQUIRED", IsActive = true });
        using var client = factory.CreateHttpsClient();
        var html = WebUtility.HtmlDecode(await client.GetStringAsync("/uzmanlik-alanlari/test-alani"));
        Assert.DoesNotContain("CONTENT_REQUIRED", html, StringComparison.Ordinal);
        Assert.Contains("Test çalışma alanı", html, StringComparison.Ordinal);
        Assert.Contains("https://cevatbiyik.av.tr/uzmanlik-alanlari/test-alani", html, StringComparison.Ordinal);
        Assert.Contains("noindex", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ArticleExcerpt_PlainTextMetadataAndSchemaAgreeWithVisibleContent()
    {
        using var factory = new PublicWebFactory();
        var article = factory.Articles.Items.Single(x => x.Id == 1);
        article.Excerpt = "<p>TEST_ONLY summary</p>";
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/makaleler/test-makalesi");
        Assert.Contains("name=\"description\" content=\"TEST_ONLY summary\"", html, StringComparison.Ordinal);
        Assert.Contains("\"description\":\"TEST_ONLY summary\"", html, StringComparison.Ordinal);
        article.Excerpt = "CONTENT_REQUIRED";
        html = await client.GetStringAsync("/makaleler/test-makalesi");
        Assert.DoesNotContain("CONTENT_REQUIRED", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ReviewedLegalBody_ProvidesUniquePlainTextDescription()
    {
        using var factory = new PublicWebFactory();
        factory.Pages.Seed(new PageContent { Id = 20, Path = "/kvkk", Title = "TEST_ONLY notice", Body = "<p>TEST_ONLY reviewed legal description</p>", IsActive = true, IsReviewed = true });
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/kvkk");
        Assert.Contains("name=\"description\" content=\"TEST_ONLY reviewed legal description\"", html, StringComparison.Ordinal);
    }
}
