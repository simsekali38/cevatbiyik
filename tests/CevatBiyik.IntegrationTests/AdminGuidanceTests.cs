using System.Net;
using System.Text.RegularExpressions;
using CevatBiyik.Domain.Entities;

namespace CevatBiyik.IntegrationTests;

public sealed class AdminGuidanceTests
{
    [Fact]
    public async Task Guide_RequiresAdmin_AndBoundsStepSelection()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync();
        using var client = factory.CreateHttpsClient();
        var anonymous = await client.GetAsync("/admin/guide");
        Assert.Equal(HttpStatusCode.Redirect, anonymous.StatusCode);
        var html = await client.GetStringAsync("/admin/login");
        var token = WebUtility.HtmlDecode(Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value);
        await client.PostAsync("/admin/login", new FormUrlEncodedContent(new Dictionary<string, string>
        { ["UserName"] = "admin", ["Password"] = AdminWebFactory.Password, ["__RequestVerificationToken"] = token }));
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/admin/guide?step=9")).StatusCode);
        var guide = await client.GetStringAsync("/admin/guide?step=3");
        Assert.Contains("aria-current=\"step\"", guide, StringComparison.Ordinal);
        Assert.Contains("Hukuki metinler", WebUtility.HtmlDecode(guide), StringComparison.Ordinal);
        Assert.Contains("0 / 8", guide, StringComparison.Ordinal);
        Assert.DoesNotContain("Smtp.Password", guide, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Contact_PrivacyModal_OnlyUsesReviewedPublicContent_AndSanitizesLegacyHtml()
    {
        using var factory = new PublicWebFactory();
        factory.Pages.Seed(new PageContent { Id = 1, Path = "/kvkk", Title = "TEST_ONLY privacy", Body = "<p>TEST_ONLY reviewed</p><script>unsafe()</script>", IsActive = true, IsReviewed = true });
        using var client = factory.CreateHttpsClient();
        var html = await client.GetStringAsync("/iletisim");
        Assert.Contains("data-open-dialog=\"kvkk-notice\"", html, StringComparison.Ordinal);
        Assert.Contains("id=\"kvkk-notice\"", html, StringComparison.Ordinal);
        Assert.Contains("TEST_ONLY reviewed", html, StringComparison.Ordinal);
        Assert.DoesNotContain("unsafe()", html, StringComparison.Ordinal);
        var token = WebUtility.HtmlDecode(Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value);
        var invalid = await client.PostAsync("/iletisim", new FormUrlEncodedContent(new Dictionary<string, string>
        { ["Form.Name"] = "TEST_ONLY preserved", ["Form.Email"] = "invalid", ["Form.Message"] = "TEST_ONLY unsent", ["__RequestVerificationToken"] = token }));
        Assert.Equal(HttpStatusCode.OK, invalid.StatusCode);
        var invalidHtml = await invalid.Content.ReadAsStringAsync();
        Assert.Contains("id=\"kvkk-notice\"", invalidHtml, StringComparison.Ordinal);
        Assert.Contains("TEST_ONLY reviewed", invalidHtml, StringComparison.Ordinal);
        Assert.Contains("TEST_ONLY preserved", invalidHtml, StringComparison.Ordinal);
        Assert.Empty(factory.Messages.Items);
        factory.Pages.Items[0].IsReviewed = false;
        var unreviewed = await client.GetStringAsync("/iletisim");
        Assert.DoesNotContain("TEST_ONLY reviewed", unreviewed, StringComparison.Ordinal);
        Assert.Contains("CONTENT_REQUIRED", unreviewed, StringComparison.Ordinal);
    }
}
