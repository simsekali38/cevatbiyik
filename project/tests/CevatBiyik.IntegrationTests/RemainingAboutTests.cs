using CevatBiyik.Domain.Entities;

namespace CevatBiyik.IntegrationTests;

public sealed class RemainingAboutTests
{
    [Fact]
    public async Task MissingProfile_ShowsNeutralState_WithoutPublicPlaceholderFacts()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/hakkimda");
        Assert.DoesNotContain("CONTENT_REQUIRED", html, StringComparison.Ordinal);
        Assert.DoesNotContain("biography-title", html, StringComparison.Ordinal);
        Assert.Contains("about-portrait-placeholder", html, StringComparison.Ordinal);
        Assert.Contains("noindex", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ReviewedProfile_RendersOnlyExistingCmsFields_AndRetainsSafeHtml()
    {
        using var factory = new PublicWebFactory();
        factory.Profiles.Seed(new LawyerProfile
        {
            Id = 1, Key = "primary", Biography = "<h3>TEST_ONLY biography</h3><p>TEST_ONLY history</p>",
            Vision = "TEST_ONLY vision", BarAssociation = "TEST_ONLY bar", IsActive = true, IsReviewed = true
        });
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/hakkimda");
        Assert.Contains("<h3>TEST_ONLY biography</h3>", html, StringComparison.Ordinal);
        Assert.Contains("TEST_ONLY vision", html, StringComparison.Ordinal);
        Assert.Contains("TEST_ONLY bar", html, StringComparison.Ordinal);
        Assert.Contains("https://cevatbiyik.av.tr/hakkimda", html, StringComparison.Ordinal);
    }
}
