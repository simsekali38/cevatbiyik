using System.Net;
using System.Text.RegularExpressions;

namespace CevatBiyik.IntegrationTests;

public sealed partial class DisabledDocumentsTests
{
    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex Token();

    [Fact]
    public async Task DisabledModule_HasNoRoutesOrUi_AndCannotManagePrivateFiles()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using var client = factory.CreateHttpsClient();
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/admin/documents/download/1")).StatusCode);
        var login = await client.GetStringAsync("/admin/login");
        var token = WebUtility.HtmlDecode(Token().Match(login).Groups[1].Value);
        await client.PostAsync("/admin/login", new FormUrlEncodedContent(new Dictionary<string, string>
        { ["UserName"] = "admin", ["Password"] = AdminWebFactory.Password, ["__RequestVerificationToken"] = token }));
        foreach (var path in new[] { "/admin", "/admin/guide", "/admin/guide?step=4", "/admin/files", "/admin/media" })
        {
            var html = await client.GetStringAsync(path);
            Assert.DoesNotContain("/admin/documents", html, StringComparison.Ordinal);
            Assert.DoesNotContain("documents=true", html, StringComparison.Ordinal);
            Assert.DoesNotContain("Özel belgeler", WebUtility.HtmlDecode(html), StringComparison.Ordinal);
        }
        foreach (var path in new[] { "/admin/documents", "/admin/documents?deleted=true", "/admin/documents/download/1", "/admin/files?documents=true" })
        {
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(path)).StatusCode);
        }
        token = WebUtility.HtmlDecode(Token().Match(await client.GetStringAsync("/admin/files")).Groups[1].Value);
        foreach (var path in new[] { "/admin/documents/upload", "/admin/documents/delete/1", "/admin/documents/restore/1", "/admin/files/quarantine" })
        {
            var response = await client.PostAsync(path, new FormUrlEncodedContent(new Dictionary<string, string>
            { ["__RequestVerificationToken"] = token, ["documents"] = "true", ["confirmed"] = "true", ["path"] = "TEST_ONLY.pdf" }));
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
