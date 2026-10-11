namespace CevatBiyik.IntegrationTests;

public sealed class RichContentFlowTests
{
    [Fact]
    public async Task PublicArticle_RendersSafeFormatting_AndRemovesLegacyXss()
    {
        await using var factory = new PublicWebFactory();
        factory.Articles.Items.Single(x => x.Id == 1).Content = "<h2>Test başlığı</h2><p onclick=\"alert(98765)\"><strong>Test vurgusu</strong></p><script>alert(98765)</script>";
        using var client = factory.CreateHttpsClient();
        var response = await client.GetAsync("/makaleler/test-makalesi");
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("<strong>Test vurgusu</strong>", html);
        Assert.DoesNotContain("alert(98765)", html);
        Assert.Contains("rich-content", html);
    }
}
