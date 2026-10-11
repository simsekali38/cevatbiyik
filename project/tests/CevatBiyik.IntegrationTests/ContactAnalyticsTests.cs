using System.Net;
using System.Text.RegularExpressions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Email;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;

public sealed partial class ContactAnalyticsTests
{
    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex TokenPattern();
    private static async Task<HttpResponseMessage> Post(HttpClient client, string page, string target, Dictionary<string, string> values)
    {
        var html = await client.GetStringAsync(page);
        values["__RequestVerificationToken"] = WebUtility.HtmlDecode(TokenPattern().Match(html).Groups[1].Value);
        return await client.PostAsync(target, new FormUrlEncodedContent(values));
    }
    private static Dictionary<string, string> Form() => new()
    { ["Form.Name"] = "Test", ["Form.Email"] = "test@example.test", ["Form.Message"] = "TEST_ONLY_PRIVATE_MESSAGE", ["Form.Consent"] = "true" };
    private static void Legal(PublicWebFactory factory)
    {
        foreach (var path in new[] { "/kvkk", "/cerez-politikasi" })
        {
            factory.Pages.Seed(new PageContent { Id = factory.Pages.Items.Count + 1, Path = path, Title = "Test legal page", Body = "TEST_ONLY_REVIEWED_LEGAL_CONTENT", IsActive = true, IsReviewed = true });
        }
    }
    private static Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> Analytics(PublicWebFactory factory, string? id = "GTM-TEST123", string version = "v1", bool enabled = true) =>
        factory.WithWebHostBuilder(builder => builder.ConfigureAppConfiguration((_, c) => c.AddInMemoryCollection(new Dictionary<string, string?>
        { ["Analytics:Enabled"] = enabled.ToString(), ["Analytics:LegalReviewConfirmed"] = "true", ["Analytics:GtmId"] = id,
          ["Analytics:Ga4Id"] = "", ["Analytics:ConsentVersion"] = version, ["Analytics:Notice"] = "TEST_ONLY analytics provider and retention notice reviewed." })));
    private static HttpClient Client(Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> factory) => factory.CreateClient(new()
    { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false, HandleCookies = true });

    [Fact]
    public async Task Honeypot_SilentPrgWithoutDatabaseOrAnalyticsEvent()
    {
        using var factory = new PublicWebFactory(); Legal(factory);
        using var configured = Analytics(factory); using var client = Client(configured);
        await Post(client, "/", "/cerez-tercihi", new() { ["choice"] = "allow" });
        var form = Form(); form["Website"] = "https://spam.invalid";
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/iletisim", "/iletisim", form)).StatusCode);
        Assert.Empty(factory.Messages.Items);
        Assert.Contains("data-contact-recorded=\"false\"", await client.GetStringAsync("/iletisim"), StringComparison.Ordinal);
    }
    [Fact]
    public async Task SuccessfulContact_EmitsMarkerOnceOnlyAfterDatabaseSave()
    {
        using var factory = new PublicWebFactory(); Legal(factory);
        using var configured = Analytics(factory); using var client = Client(configured);
        await Post(client, "/", "/cerez-tercihi", new() { ["choice"] = "allow" });
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/iletisim", "/iletisim", Form())).StatusCode);
        Assert.Single(factory.Messages.Items);
        Assert.Contains("data-contact-recorded=\"true\"", await client.GetStringAsync("/iletisim"), StringComparison.Ordinal);
        Assert.Contains("data-contact-recorded=\"false\"", await client.GetStringAsync("/iletisim"), StringComparison.Ordinal);
    }
    [Fact]
    public async Task Consent_OptInRejectWithdrawAndSecurityHeaders()
    {
        using var factory = new PublicWebFactory(); Legal(factory);
        using var configured = Analytics(factory); using var client = Client(configured);
        var before = await client.GetAsync("/"); var initial = await before.Content.ReadAsStringAsync();
        if (Environment.GetEnvironmentVariable("CEVATBIYIK_CMS_PREVIEW") == "1")
        {
            var env = configured.Services.GetRequiredService<IWebHostEnvironment>();
            var root = Path.Combine(RepositoryPaths.Artifacts, "cms-preview");
            Directory.CreateDirectory(root);
            await File.WriteAllTextAsync(Path.Combine(root, "consent.html"), initial);
            await File.WriteAllTextAsync(Path.Combine(root, "contact-consent.html"), await client.GetStringAsync("/iletisim"));
            foreach (var directory in new[] { "css", "js", "lib/bootstrap", "images/brand" })
            {
                foreach (var file in Directory.GetFiles(Path.Combine(env.WebRootPath, directory), "*", SearchOption.AllDirectories))
                {
                    var target = Path.Combine(root, directory, Path.GetRelativePath(Path.Combine(env.WebRootPath, directory), file));
                    Directory.CreateDirectory(Path.GetDirectoryName(target)!); File.Copy(file, target, true);
                }
            }
        }
        Assert.Contains("Analitiği reddet", WebUtility.HtmlDecode(initial), StringComparison.Ordinal);
        Assert.DoesNotContain("/js/analytics.min.js", initial, StringComparison.Ordinal);
        Assert.DoesNotContain("googletagmanager.com", before.Headers.GetValues("Content-Security-Policy").Single(), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/cerez-tercihi", new FormUrlEncodedContent(new Dictionary<string,string> { ["choice"] = "allow" }))).StatusCode);
        var allowed = await Post(client, "/", "/cerez-tercihi", new() { ["choice"] = "allow", ["returnUrl"] = "https://evil.invalid" });
        Assert.Equal("/", allowed.Headers.Location!.OriginalString);
        var cookie = allowed.Headers.GetValues("Set-Cookie").Single(x => x.StartsWith("__Host-CevatBiyik.Consent=", StringComparison.Ordinal));
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase); Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=strict", cookie, StringComparison.OrdinalIgnoreCase); Assert.DoesNotContain("domain=", cookie, StringComparison.OrdinalIgnoreCase);
        var after = await client.GetAsync("/"); var html = await after.Content.ReadAsStringAsync();
        Assert.Contains("/js/analytics.min.js", html, StringComparison.Ordinal); Assert.Contains("data-provider=\"gtm\"", html, StringComparison.Ordinal);
        var csp = after.Headers.GetValues("Content-Security-Policy").Single();
        Assert.Contains("https://www.googletagmanager.com", csp, StringComparison.Ordinal); Assert.DoesNotContain("unsafe-inline", csp, StringComparison.Ordinal);
        Assert.DoesNotContain("unsafe-eval", csp, StringComparison.Ordinal); Assert.True(after.Headers.CacheControl!.NoStore);
        var nonce = WebUtility.HtmlDecode(Regex.Match(html, "nonce=\"([^\"]+)\"[^>]*data-provider").Groups[1].Value);
        Assert.NotEmpty(nonce); Assert.Contains("'nonce-" + nonce + "'", csp, StringComparison.Ordinal);
        await Post(client, "/", "/cerez-tercihi", new() { ["choice"] = "deny" });
        Assert.DoesNotContain("/js/analytics.min.js", await client.GetStringAsync("/"), StringComparison.Ordinal);
    }
    [Theory]
    [InlineData(false, "GTM-TEST123", true)]
    [InlineData(true, "GTM-TEST123", false)]
    [InlineData(true, "GTM-TEST123<script>", true)]
    public async Task Analytics_FailsClosedWithoutLegalContentEnabledFlagOrValidId(bool legal, string id, bool enabled)
    {
        using var factory = new PublicWebFactory(); if (legal) { Legal(factory); }
        using var configured = Analytics(factory, id, enabled: enabled); using var client = Client(configured);
        await Post(client, "/", "/cerez-tercihi", new() { ["choice"] = "allow" });
        var response = await client.GetAsync("/");
        Assert.DoesNotContain("/js/analytics.min.js", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.DoesNotContain("googletagmanager.com", response.Headers.GetValues("Content-Security-Policy").Single(), StringComparison.Ordinal);
    }
    [Fact]
    public async Task TamperedCookieAndInvalidChoiceRemainDisabled()
    {
        using var factory = new PublicWebFactory(); Legal(factory);
        using var configured = Analytics(factory); using var client = Client(configured);
        client.DefaultRequestHeaders.Add("Cookie", "__Host-CevatBiyik.Consent=allow");
        Assert.DoesNotContain("/js/analytics.min.js", await client.GetStringAsync("/"), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/", "/cerez-tercihi", new() { ["choice"] = "arbitrary" })).StatusCode);
    }
    [Theory]
    [InlineData("v0", 0)] [InlineData("v1", 181)] [InlineData("v1", -1)]
    public async Task ExpiredFutureOrSupersededConsentRequiresNewChoice(string version, int daysAgo)
    {
        using var factory = new PublicWebFactory(); Legal(factory);
        using var configured = Analytics(factory); using var client = Client(configured);
        var protector = configured.Services.GetRequiredService<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>()
            .CreateProtector("CevatBiyik.AnalyticsConsent.v1");
        var payload = $"{version}|allow|{DateTime.UtcNow.AddDays(-daysAgo).Ticks}";
        var cookie = Microsoft.AspNetCore.DataProtection.DataProtectionCommonExtensions.Protect(protector, payload);
        client.DefaultRequestHeaders.Add("Cookie", "__Host-CevatBiyik.Consent=" + cookie);
        var response = await client.GetAsync("/");
        Assert.DoesNotContain("/js/analytics.min.js", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.DoesNotContain("googletagmanager.com", response.Headers.GetValues("Content-Security-Policy").Single(), StringComparison.Ordinal);
    }
    [Fact]
    public async Task Ga4IsSupportedButAdminNeverContainsAnalytics()
    {
        using var factory = new PublicWebFactory(); Legal(factory);
        using var configured = Analytics(factory, null).WithWebHostBuilder(builder => builder.ConfigureAppConfiguration((_, c) => c.AddInMemoryCollection(new Dictionary<string, string?> { ["Analytics:Ga4Id"] = "G-TEST123" })));
        using var client = Client(configured);
        await Post(client, "/", "/cerez-tercihi", new() { ["choice"] = "allow" });
        Assert.Contains("data-provider=\"ga4\"", await client.GetStringAsync("/"), StringComparison.Ordinal);
        var admin = await client.GetAsync("/admin/login");
        Assert.DoesNotContain("analytics.min.js", await admin.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.DoesNotContain("googletagmanager.com", admin.Headers.GetValues("Content-Security-Policy").Single(), StringComparison.Ordinal);
    }
    [Fact]
    public async Task AdminRetryRequiresAuthorizationCsrfConfirmationAndWritesAudit()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync(); using var client = factory.CreateHttpsClient();
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.ContactMessages.Add(new ContactMessage { Name = "TEST_ONLY", Email = "test@example.test", Message = "TEST_ONLY", NotificationPending = true, NotificationAttempts = 6 });
        await db.SaveChangesAsync(); var id = await db.ContactMessages.Select(x => x.Id).SingleAsync();
        var target = $"/admin/contact-messages/retry-notification/{id}";
        Assert.Equal(HttpStatusCode.Redirect, (await client.PostAsync(target, new FormUrlEncodedContent(new Dictionary<string,string>()))).StatusCode);
        await Post(client, "/admin/login", "/admin/login", new() { ["UserName"] = "admin", ["Password"] = AdminWebFactory.Password });
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync(target, new FormUrlEncodedContent(new Dictionary<string,string>()))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Post(client, "/admin/contact-messages", target, new())).StatusCode);
        // Disabled SMTP is a safe rejection, not a requeue.
        await Post(client, "/admin/contact-messages", target, new() { ["confirmed"] = "true" });
        db.ChangeTracker.Clear(); Assert.Equal(6, (await db.ContactMessages.SingleAsync()).NotificationAttempts);
        Assert.False(await db.AuditLogs.AnyAsync(x => x.Action == "ContactNotificationRetried"));
        var service = new CevatBiyik.Application.Services.ContactService(new EfRepository<ContactMessage>(db), new CevatBiyik.Application.Validation.CreateContactMessageValidator(), TimeProvider.System, new SmtpNotificationOptions { Enabled = true });
        await scope.ServiceProvider.GetRequiredService<IAdminOperation>().ExecuteAsync(SecurityEvent.ContactNotificationRetried, "TEST_ONLY", () => service.RetryNotificationAsync(id));
        Assert.Equal(0, (await db.ContactMessages.SingleAsync()).NotificationAttempts);
        Assert.True(await db.AuditLogs.AnyAsync(x => x.Action == "ContactNotificationRetried"));
    }
    [Fact]
    public async Task NotificationStore_ClaimsRetriesRecoversAndStopsDeletedOrExhaustedJobs()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync();
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var row = new ContactMessage { Name = "Test", Email = "test@example.test", Message = "TEST_ONLY", NotificationPending = true };
        db.ContactMessages.Add(row); await db.SaveChangesAsync();
        var time = new MutableClock(); var store = new ContactNotificationStore(db, time);
        var job = (await store.ClaimAsync(default))!; Assert.NotNull(job); Assert.Equal(1, job.Attempt);
        Assert.Null(await store.ClaimAsync(default));
        await store.CompleteAsync(job, false, default); Assert.Null(await store.ClaimAsync(default));
        time.Now += TimeSpan.FromMinutes(3);
        var retry = (await store.ClaimAsync(default))!; Assert.Equal(2, retry.Attempt);
        time.Now += TimeSpan.FromMinutes(3); // lease expires after crash
        var recovered = (await store.ClaimAsync(default))!; Assert.Equal(3, recovered.Attempt);
        await store.CompleteAsync(retry, true, default); Assert.True(await store.IsCurrentAsync(recovered, default));
        await store.CompleteAsync(recovered, true, default); Assert.Null(await store.ClaimAsync(default));
        await db.Entry(row).ReloadAsync(); Assert.NotNull(row.NotificationSentAt); Assert.False(row.NotificationPending);
        row.NotificationPending = true; row.NotificationAttempts = 6; await db.SaveChangesAsync(); Assert.Null(await store.ClaimAsync(default));
        row.NotificationAttempts = 0; row.IsDeleted = true; await db.SaveChangesAsync(); Assert.Null(await store.ClaimAsync(default));
    }
    private sealed class MutableClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = DateTimeOffset.UtcNow;
        public override DateTimeOffset GetUtcNow() => Now;
    }
}
