using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CevatBiyik.IntegrationTests;

public partial class ContactFlowTests
{
    [Fact]
    public async Task Submit_WithoutCsrfTokenIsRejectedAndNothingSaved()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var response = await client.PostAsync("/iletisim", new FormUrlEncodedContent(ValidForm()));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(factory.Messages.Items);
    }

    [Fact]
    public async Task Submit_ValidTokenSavesOnceAndShowsTempDataAfterRedirect()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var form = ValidForm();
        form["__RequestVerificationToken"] = await TokenAsync(client);
        form["Form.IsActive"] = "true";
        form["Form.IsRead"] = "true";
        var response = await client.PostAsync("/iletisim", new FormUrlEncodedContent(form));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/iletisim", response.Headers.Location?.OriginalString);
        var message = Assert.Single(factory.Messages.Items);
        Assert.Equal("Test mesajı", message.Message);
        Assert.True(message.Consent);
        Assert.False(message.IsRead);
        Assert.False(message.IsActive);
        var success = WebUtility.HtmlDecode(await client.GetStringAsync("/iletisim"));
        Assert.Contains("Mesajınız kaydedildi", success, StringComparison.Ordinal);
        Assert.DoesNotContain("Mesajınız kaydedildi", WebUtility.HtmlDecode(await client.GetStringAsync("/iletisim")), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("<script>alert(1)</script>", "test@example.com", "true")]
    [InlineData("Test mesajı", "invalid-email", "true")]
    [InlineData("Test mesajı", "test@example.com", "false")]
    public async Task Submit_InvalidInputShowsEncodedFieldErrorsWithoutSaving(string message, string email, string consent)
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var form = ValidForm();
        form["__RequestVerificationToken"] = await TokenAsync(client);
        form["Form.Message"] = message;
        form["Form.Email"] = email;
        form["Form.Consent"] = consent;
        var response = await client.PostAsync("/iletisim", new FormUrlEncodedContent(form));
        var html = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("validation-summary-errors", html, StringComparison.Ordinal);
        Assert.Contains("aria-invalid=\"true\"", html, StringComparison.Ordinal);
        Assert.DoesNotContain("<script>alert(1)</script>", html, StringComparison.Ordinal);
        Assert.Empty(factory.Messages.Items);
    }

    [Fact]
    public async Task Submit_ExcessiveAttemptsAreRateLimited()
    {
        using var factory = new PublicWebFactory();
        using var client = factory.CreateHttpsClient();
        var form = ValidForm();
        form["__RequestVerificationToken"] = await TokenAsync(client);
        form["Form.Consent"] = "false";
        for (var i = 0; i < 5; i++)
        {
            Assert.Equal(HttpStatusCode.OK, (await client.PostAsync("/iletisim", new FormUrlEncodedContent(form))).StatusCode);
        }
        Assert.Equal(HttpStatusCode.TooManyRequests, (await client.PostAsync("/iletisim", new FormUrlEncodedContent(form))).StatusCode);
        Assert.Empty(factory.Messages.Items);
    }

    private static Dictionary<string, string> ValidForm() => new()
    {
        ["Form.Name"] = "Test",
        ["Form.Email"] = "test@example.com",
        ["Form.Message"] = "Test mesajı",
        ["Form.Consent"] = "true"
    };

    [Theory]
    [InlineData(false, "CONTENT_REQUIRED")]
    [InlineData(true, "CONTENT_REQUIRED")]
    public async Task ProductionForm_RequiresExplicitEnableAndApprovedNotice(bool enabled, string notice)
    {
        using var factory = new PublicWebFactory();
        using var production = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Site:ContactFormEnabled"] = enabled.ToString(),
                ["Site:ContactNotice"] = notice
            }));
        });
        using var client = production.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
        var form = ValidForm();
        form["__RequestVerificationToken"] = await TokenAsync(client);
        var response = await client.PostAsync("/iletisim", new FormUrlEncodedContent(form));
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Empty(factory.Messages.Items);
    }

    private static async Task<string> TokenAsync(HttpClient client)
    {
        var html = await client.GetStringAsync("/iletisim");
        var match = TokenPattern().Match(html);
        Assert.True(match.Success);
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex TokenPattern();
}
