using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.IntegrationTests;

public sealed class HomeHeroTests
{
    [Fact]
    public async Task DefaultHero_IsLocalOptimizedAndExplicitlyRepresentative()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/");
        Assert.Contains("representative-legal-still-life-1200.webp", html, StringComparison.Ordinal);
        Assert.Contains("home-image-caption", html, StringComparison.Ordinal);
        Assert.Contains("fetchpriority=\"high\"", html, StringComparison.Ordinal);
        Assert.DoesNotContain("public-breadcrumb", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CmsHero_UsesOnlyActiveSafeLocalMedia_AndFallsBackAfterDeletion()
    {
        using var factory = new PublicWebFactory();
        factory.Settings.Seed(new SiteSetting { Id = 101, Key = "Home.HeroMediaId", Value = "101", IsActive = true });
        var media = new MediaAsset { Id = 101, StoragePath = "images/" + new string('a', 32) + ".webp", ContentType = "image/webp", Width = 100, Height = 100, Bytes = 100, AltText = "TEST_ONLY approved image", IsActive = true };
        factory.Media.Seed(media);
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/");
        Assert.Contains("/uploads/" + media.StoragePath, html, StringComparison.Ordinal);
        Assert.DoesNotContain("representative-legal-still-life-1200.webp", html, StringComparison.Ordinal);
        media.IsDeleted = true;
        html = await client.GetStringAsync("/");
        Assert.Contains("representative-legal-still-life-1200.webp", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("https://example.test/image.webp")]
    [InlineData("../image.webp")]
    public void ActiveHeroSetting_RejectsUrlsAndInvalidIdentifiers(string value)
    {
        Assert.False(new UpdateSiteSettingValidator().Validate(new UpdateSiteSettingDto { Key = "Home.HeroMediaId", Value = value, IsActive = true }).IsValid);
    }

    [Fact]
    public async Task HomeArticleCover_DisappearsWhenCmsMediaIsDeactivated()
    {
        using var factory = new PublicWebFactory();
        var cover = new MediaAsset { Id = 120, StoragePath = "images/" + new string('b', 32) + ".webp",
            ContentType = "image/webp", Width = 800, Height = 450, AltText = "TEST_ONLY approved article cover", IsActive = true };
        factory.Articles.Items.Single(x => x.Id == 1).CoverMedia = cover;
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/");
        Assert.Contains("/uploads/" + cover.StoragePath, html, StringComparison.Ordinal);
        Assert.Contains("TEST_ONLY approved article cover", html, StringComparison.Ordinal);
        cover.IsActive = false;
        html = await client.GetStringAsync("/");
        Assert.DoesNotContain("/uploads/" + cover.StoragePath, html, StringComparison.Ordinal);
        Assert.Contains("home-article-placeholder", html, StringComparison.Ordinal);
        Assert.DoesNotContain("Gizli taslak", html, StringComparison.Ordinal);
        Assert.DoesNotContain("Gelecek yayın", html, StringComparison.Ordinal);
    }
}
