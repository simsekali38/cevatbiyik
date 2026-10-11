using System.Net;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.IntegrationTests;

public sealed class PracticeAreaDesignTests
{
    [Fact]
    public async Task OptionalPlaceholderContent_IsOmittedWithoutChangingPublication()
    {
        using var factory = new PublicWebFactory();
        factory.Areas.Seed(new PracticeArea
        {
            Id = 100, Name = "TEST_ONLY optional", Slug = "test-optional", IsActive = true,
            ShortDescription = "CONTENT_REQUIRED", Content = "CONTENT_REQUIRED"
        });
        using var client = factory.CreateHttpsClient();
        var list = await client.GetStringAsync("/uzmanlik-alanlari");
        Assert.Contains("/uzmanlik-alanlari/test-optional", list, StringComparison.Ordinal);
        var detail = await client.GetStringAsync("/uzmanlik-alanlari/test-optional");
        var main = detail[detail.IndexOf("<main", StringComparison.Ordinal)..detail.IndexOf("</main>", StringComparison.Ordinal)];
        Assert.DoesNotContain("CONTENT_REQUIRED", main, StringComparison.Ordinal);
        Assert.DoesNotContain("class=\"plain-content\"", detail, StringComparison.Ordinal);
        Assert.DoesNotContain("class=\"practice-lead\"", detail, StringComparison.Ordinal);
        Assert.Contains("class=\"practice-contact\"", detail, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CmsSectionHeadings_AreSanitized_AndDraftAndDeletedAreasStayPrivate()
    {
        using var factory = new PublicWebFactory();
        factory.Areas.Seed(new PracticeArea
        {
            Id = 101, Name = "TEST_ONLY structured", Slug = "test-structured", IsActive = true,
            Content = "<h2>TEST_ONLY scope</h2><p>TEST_ONLY body</p><h3>TEST_ONLY approach</h3><script>alert(1)</script>"
        });
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/uzmanlik-alanlari/test-structured");
        Assert.Contains("<h2>TEST_ONLY scope</h2>", html, StringComparison.Ordinal);
        Assert.Contains("<h3>TEST_ONLY approach</h3>", html, StringComparison.Ordinal);
        Assert.DoesNotContain("<script>alert(1)</script>", html, StringComparison.Ordinal);
        Assert.Contains("https://cevatbiyik.av.tr/uzmanlik-alanlari/test-structured", html, StringComparison.Ordinal);
        Assert.Contains("BreadcrumbList", html, StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/uzmanlik-alanlari/gizli-alan")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/uzmanlik-alanlari/silinen-alan")).StatusCode);
    }
}
