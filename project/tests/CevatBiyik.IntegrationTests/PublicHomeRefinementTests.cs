using System.Net;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.IntegrationTests;

public sealed class PublicHomeRefinementTests
{
    [Fact]
    public async Task Homepage_KeepsPublishedLinksWithoutExposingPlaceholdersOrUnreviewedProfile()
    {
        using var factory = new PublicWebFactory();
        factory.Areas.Items.Single(x => x.Id == 1).ShortDescription = "CONTENT_REQUIRED";
        factory.Articles.Items.Single(x => x.Id == 1).Excerpt = "CONTENT_REQUIRED";
        factory.Profiles.Seed(new LawyerProfile
        {
            Id = 1, Biography = "TEST_ONLY_UNREVIEWED_PROFILE", Vision = "TEST_ONLY", BarAssociation = "TEST_ONLY",
            IsActive = true, IsReviewed = false
        });
        using var client = factory.CreateHttpsClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("CONTENT_REQUIRED", html, StringComparison.Ordinal);
        Assert.DoesNotContain("TEST_ONLY_UNREVIEWED_PROFILE", html, StringComparison.Ordinal);
        Assert.Contains("/makaleler/test-makalesi", html, StringComparison.Ordinal);
        Assert.Contains("/uzmanlik-alanlari/test-alani", html, StringComparison.Ordinal);
    }
}
