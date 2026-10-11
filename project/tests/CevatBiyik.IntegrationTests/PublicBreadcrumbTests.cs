using CevatBiyik.Web.Models;

namespace CevatBiyik.IntegrationTests;

public sealed class PublicBreadcrumbTests
{
    [Theory]
    [InlineData("/sss")]
    [InlineData("/makaleler")]
    [InlineData("/makaleler/test-makalesi")]
    [InlineData("/uzmanlik-alanlari")]
    [InlineData("/uzmanlik-alanlari/test-alani")]
    [InlineData("/hakkimda")]
    [InlineData("/iletisim")]
    [InlineData("/kvkk")]
    [InlineData("/cerez-politikasi")]
    [InlineData("/yasal-uyari")]
    public async Task PublicPages_HaveOneSharedBreadcrumbBeforeTheHeading(string path)
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync(path);
        var start = html.IndexOf("public-breadcrumb", StringComparison.Ordinal);
        Assert.True(start > 0 && start < html.IndexOf("<h1", StringComparison.Ordinal));
        Assert.Equal(start, html.LastIndexOf("public-breadcrumb", StringComparison.Ordinal));
        Assert.Contains("breadcrumb-item active", html, StringComparison.Ordinal);
    }

    [Fact]
    public void Hierarchy_IsNotCreatedForHomepageOrUnknownRoutes()
    {
        Assert.Empty(PublicBreadcrumb.Create("/", "Ana sayfa", null));
        Assert.Empty(PublicBreadcrumb.Create("/unknown", "Not found", null));
        var links = PublicBreadcrumb.Create("/makaleler/a-slug", "TEST_ONLY title", null);
        Assert.Equal(new[] { "/", "/makaleler", null }, links.Select(x => x.Url));
        Assert.Equal("TEST_ONLY title", links[^1].Title);
    }
}
