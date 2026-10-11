using CevatBiyik.Domain.Entities;

namespace CevatBiyik.IntegrationTests;

public sealed class RemainingPublicPageTests
{
    [Theory]
    [InlineData("/sss")]
    [InlineData("/kvkk")]
    [InlineData("/cerez-politikasi")]
    [InlineData("/yasal-uyari")]
    [InlineData("/iletisim")]
    public async Task RemainingPages_KeepBreadcrumbAndCanonical(string path)
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync(path);
        Assert.Contains("content-breadcrumb", html, StringComparison.Ordinal);
        Assert.Contains("aria-current=\"page\"", html, StringComparison.Ordinal);
        Assert.Contains("https://cevatbiyik.av.tr" + path, html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task LegalPages_OnlyRenderReviewedCmsText()
    {
        using var factory = new PublicWebFactory();
        factory.Pages.Seed(new PageContent { Id = 1, Path = "/kvkk", Title = "TEST_ONLY legal", Body = "TEST_ONLY_UNREVIEWED", IsActive = true });
        using var client = factory.CreateHttpsClient();
        Assert.DoesNotContain("TEST_ONLY_UNREVIEWED", await client.GetStringAsync("/kvkk"), StringComparison.Ordinal);
        factory.Pages.Items.Single().IsReviewed = true;
        Assert.Contains("TEST_ONLY_UNREVIEWED", await client.GetStringAsync("/kvkk"), StringComparison.Ordinal);
    }

    [Fact]
    public async Task ContactDetails_HideMissingFacts_WhileFormRetainsCsrfAndConsent()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/iletisim");
        var panel = html[html.IndexOf("<aside", StringComparison.Ordinal)..html.IndexOf("</aside>", StringComparison.Ordinal)];
        Assert.DoesNotContain("CONTENT_REQUIRED", panel, StringComparison.Ordinal);
        Assert.Contains("test@example.com", panel, StringComparison.Ordinal);
        Assert.Contains("name=\"__RequestVerificationToken\"", html, StringComparison.Ordinal);
        Assert.Contains("name=\"Form.Consent\"", html, StringComparison.Ordinal);
        Assert.Contains("id=\"kvkk-notice\"", html, StringComparison.Ordinal);
    }
}
