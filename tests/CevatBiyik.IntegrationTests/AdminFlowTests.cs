using System.Net;
using System.Text.RegularExpressions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Identity;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;

public sealed partial class AdminFlowTests
{
    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex TokenPattern();

    private static async Task<HttpResponseMessage> PostAsync(HttpClient client, string form, string path, Dictionary<string, string> values)
    {
        var html = await client.GetStringAsync(form);
        values["__RequestVerificationToken"] = WebUtility.HtmlDecode(TokenPattern().Match(html).Groups[1].Value);
        return await client.PostAsync(path, new FormUrlEncodedContent(values));
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client, string name = "admin", string? password = null, string? returnUrl = null) =>
        PostAsync(client, "/admin/login", "/admin/login", new()
        { ["UserName"] = name, ["Password"] = password ?? AdminWebFactory.Password, ["ReturnUrl"] = returnUrl ?? "/admin" });

    [Fact]
    public async Task AnonymousAdminRoutes_AreProtected_LoginIsNoStoreNoIndex()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using var client = factory.CreateHttpsClient();
        foreach (var path in new[] { "/admin", "/admin/articles", "/admin/articles/create", "/admin/practice-areas", "/admin/contact-messages", "/admin/settings" })
        {
            var response = await client.GetAsync(path);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("https://localhost/admin/login", response.Headers.Location!.ToString(), StringComparison.Ordinal);
        }
        var login = await client.GetAsync("/admin/login");
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Contains("noindex", login.Headers.GetValues("X-Robots-Tag").Single(), StringComparison.Ordinal);
        Assert.True(login.Headers.CacheControl?.NoStore);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/admin/login", new FormUrlEncodedContent(new Dictionary<string, string>()))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/admin/register")).StatusCode);
    }

    [Fact]
    public async Task LoginCookie_SafeRedirect_DashboardAndLogout()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using var client = factory.CreateHttpsClient();
        var login = await LoginAsync(client, returnUrl: "https://attacker.invalid/");
        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);
        Assert.Equal("/admin", login.Headers.Location!.OriginalString);
        var cookie = login.Headers.GetValues("Set-Cookie").Single(x => x.StartsWith("__Host-CevatBiyik.Admin=", StringComparison.Ordinal));
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=strict", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("path=/", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("domain=", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("expires=", cookie, StringComparison.OrdinalIgnoreCase);
        foreach (var path in new[] { "/admin", "/admin/articles", "/admin/practice-areas", "/admin/contact-messages", "/admin/settings" })
        {
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync(path)).StatusCode);
        }

        Assert.Equal(HttpStatusCode.MethodNotAllowed, (await client.GetAsync("/admin/logout")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/admin/logout", new FormUrlEncodedContent(new Dictionary<string, string>()))).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin", "/admin/logout", new())).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/admin")).StatusCode);
    }

    [Fact]
    public async Task ExpiredOrDisabledAdminSession_CannotDownloadPrivateDocuments()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using var client = factory.CreateHttpsClient();
        using var login = await LoginAsync(client);
        var cookie = login.Headers.GetValues("Set-Cookie").Single(x => x.StartsWith("__Host-CevatBiyik.Admin=", StringComparison.Ordinal));
        var options = factory.Services.GetRequiredService<Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions>>()
            .Get(IdentityConstants.ApplicationScheme);
        var ticket = options.TicketDataFormat.Unprotect(cookie.Split(';')[0].Split('=', 2)[1])!;
        Assert.Equal(TimeSpan.FromMinutes(30), ticket.Properties.ExpiresUtc - ticket.Properties.IssuedUtc);
        Assert.False(options.SlidingExpiration);
        ticket.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(-1);
        ticket.Properties.IssuedUtc = ticket.Properties.ExpiresUtc.Value.AddMinutes(-30);
        using var expired = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false, HandleCookies = false });
        using var request = new HttpRequestMessage(HttpMethod.Get, "/admin/documents/download/1");
        request.Headers.Add("Cookie", "__Host-CevatBiyik.Admin=" + options.TicketDataFormat.Protect(ticket));
        using var rejected = await expired.SendAsync(request);
        Assert.Equal(HttpStatusCode.Redirect, rejected.StatusCode);
        Assert.StartsWith("https://localhost/admin/login", rejected.Headers.Location!.ToString(), StringComparison.Ordinal);
        using (var scope = factory.Services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = (await users.FindByNameAsync("admin"))!;
            user.IsActive = false;
            Assert.True((await users.UpdateAsync(user)).Succeeded);
        }
        using var disabled = await client.GetAsync("/admin/documents/download/1");
        Assert.Equal(HttpStatusCode.Redirect, disabled.StatusCode);
        Assert.StartsWith("https://localhost/admin/login", disabled.Headers.Location!.ToString(), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("reader")]
    [InlineData("disabled")]
    [InlineData("twofactor")]
    [InlineData("missing")]
    public async Task IneligibleAccounts_CannotSignIn(string name)
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using var client = factory.CreateHttpsClient();
        var response = await LoginAsync(client, name);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Giriş yapılamadı", WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync()), StringComparison.Ordinal);
        Assert.DoesNotContain(AdminWebFactory.Password, await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/admin")).StatusCode);
    }

    [Fact]
    public async Task Lockout_RoleRevocation_AndRateLimit()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using var client = factory.CreateHttpsClient();
        for (var attempt = 0; attempt < 5; attempt++)
        {
            await LoginAsync(client, "locked", "Wrong-Password9!");
        }

        using (var scope = factory.Services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            Assert.True(await users.IsLockedOutAsync((await users.FindByNameAsync("locked"))!));
        }
        Assert.Equal(HttpStatusCode.OK, (await LoginAsync(client, "locked")).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await LoginAsync(client)).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await users.RemoveFromRoleAsync((await users.FindByNameAsync("admin"))!, AdminAccess.Role);
        }
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/admin")).StatusCode);
        for (var attempt = 0; attempt < 3; attempt++)
        {
            await LoginAsync(client, "missing");
        }

        Assert.Equal(HttpStatusCode.TooManyRequests, (await LoginAsync(client, "missing")).StatusCode);
    }

    [Fact]
    public async Task Crud_Validation_Publishing_SoftDelete_AndTransactionalAudit()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using var client = factory.CreateHttpsClient();
        await LoginAsync(client);
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/practice-areas/create", "/admin/practice-areas/save", new()
        { ["Name"] = "Test alanı", ["Content"] = "Test alan açıklaması", ["SortOrder"] = "1", ["IsActive"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await PostAsync(client, "/admin/articles/create", "/admin/articles/save", new()
        { ["Title"] = "", ["Content"] = "" })).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/articles/create", "/admin/articles/save", new()
        { ["Title"] = "Test makalesi", ["Content"] = "Test makale içeriği", ["IsActive"] = "false", ["Id"] = "999", ["IsDeleted"] = "true" })).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var article = await db.Articles.SingleAsync();
            Assert.False(article.IsDeleted);
            Assert.False(article.IsActive);
            Assert.NotEqual(999, article.Id);
        }
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/articles/edit/1", "/admin/articles/save/1", new()
        { ["Title"] = "Yeni başlık", ["Content"] = "Yeni test içeriği", ["IsActive"] = "true", ["PublishedAt"] = "2020-01-01T12:00", ["PracticeAreaId"] = "1" })).StatusCode);
        Assert.Contains("Yeni başlık", WebUtility.HtmlDecode(await client.GetStringAsync("/makaleler/test-makalesi")), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/settings/create", "/admin/settings/save", new()
        { ["Key"] = "Site.Description", ["Value"] = "Test SEO açıklaması", ["IsActive"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await PostAsync(client, "/admin/settings/create", "/admin/settings/save", new()
        { ["Key"] = "Smtp.Password", ["Value"] = "TEST_FORBIDDEN", ["IsActive"] = "true" })).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.ContactMessages.Add(new ContactMessage { Name = "Test kişi", Email = "test@example.test", Message = "Test mesaj", Consent = true });
            await db.SaveChangesAsync();
        }
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/contact-messages", "/admin/contact-messages/read/1", new() { ["isRead"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/contact-messages", "/admin/contact-messages/read/1", new() { ["isRead"] = "false" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await PostAsync(client, "/admin/articles", "/admin/articles/delete/1", new())).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/articles", "/admin/articles/delete/1", new() { ["confirmed"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/makaleler/test-makalesi")).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.True((await db.Articles.IgnoreQueryFilters().SingleAsync()).IsDeleted);
            Assert.Null((await db.ContactMessages.SingleAsync()).ProcessedAt);
            Assert.Contains(await db.AuditLogs.ToListAsync(), x => x.Action == "ArticleDeleted");
            Assert.DoesNotContain(await db.AuditLogs.ToListAsync(), x => x.Action.Contains(AdminWebFactory.Password, StringComparison.Ordinal));
        }
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/practice-areas/edit/1", "/admin/practice-areas/save/1", new()
        { ["Name"] = "Güncel alan", ["Content"] = "Güncel açıklama", ["IsActive"] = "false" })).StatusCode);
        Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/settings/edit/1", "/admin/settings/save/1", new()
        { ["Key"] = "Site.Description", ["Value"] = "Güncel SEO açıklaması", ["IsActive"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/admin/settings/delete/1", new FormUrlEncodedContent(new Dictionary<string, string> { ["confirmed"] = "true" }))).StatusCode);
        foreach (var path in new[] { "practice-areas", "contact-messages", "settings" })
        {
            Assert.Equal(HttpStatusCode.Redirect, (await PostAsync(client, "/admin/" + path, "/admin/" + path + "/delete/1", new() { ["confirmed"] = "true" })).StatusCode);
        }
    }

    [Fact]
    public async Task AuditAppendOnly_AndFailedAuditRollsBackMutation()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var operation = scope.ServiceProvider.GetRequiredService<IAdminOperation>();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => operation.ExecuteAsync((SecurityEvent)999, "test-actor", async () =>
            {
                db.PracticeAreas.Add(new PracticeArea { Name = "Rolled back", Slug = "rolled-back", Content = "Test" });
                await db.SaveChangesAsync();
            }));
        }
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.Empty(await db.PracticeAreas.ToListAsync());
            var audit = scope.ServiceProvider.GetRequiredService<IAuditLogService>();
            await audit.WriteAsync(SecurityEvent.LoginFailed, null);
            var entry = await db.AuditLogs.SingleAsync();
            entry.Action = "tampered";
            await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
        }
    }

    [Fact]
    public async Task LegacyPrivateSettings_AreNotDisplayedEditedOrDeleted()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.SiteSettings.Add(new SiteSetting { Key = "Smtp.Password", Value = "TEST_ONLY_PRIVATE_LEGACY", IsActive = true });
            await db.SaveChangesAsync();
        }
        using var client = factory.CreateHttpsClient();
        await LoginAsync(client);
        Assert.DoesNotContain("TEST_ONLY_PRIVATE_LEGACY", await client.GetStringAsync("/admin/settings"), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/admin/settings/edit/1")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await PostAsync(client, "/admin/settings/create", "/admin/settings/save/1", new()
        { ["Key"] = "Site.Name", ["Value"] = "Test value", ["IsActive"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await PostAsync(client, "/admin/settings", "/admin/settings/delete/1", new()
        { ["confirmed"] = "true" })).StatusCode);
    }

    [Fact]
    public async Task Bootstrap_EnforcesPasswordPolicy_IsIdempotent_AndDoesNotPromoteExistingUser()
    {
        using var factory = new AdminWebFactory();
        await factory.InitializeAsync();
        using var scope = factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var audit = scope.ServiceProvider.GetRequiredService<IAuditLogService>();
        Assert.False((await users.CreateAsync(new ApplicationUser { UserName = "weak", Email = "weak@example.test" }, "short")).Succeeded);
        var values = new Dictionary<string, string?>
        { ["AdminBootstrap:UserName"] = "newadmin", ["AdminBootstrap:Email"] = "newadmin@example.test", ["AdminBootstrap:Password"] = AdminWebFactory.Password };
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddInMemoryCollection(values).Build();
        await AdminProvisioner.ProvisionAsync(users, roles, audit, configuration);
        await AdminProvisioner.ProvisionAsync(users, roles, audit, configuration);
        Assert.True(await users.IsInRoleAsync((await users.FindByNameAsync("newadmin"))!, AdminAccess.Role));
        configuration["AdminBootstrap:UserName"] = "reader";
        await Assert.ThrowsAsync<InvalidOperationException>(() => AdminProvisioner.ProvisionAsync(users, roles, audit, configuration));
        Assert.False(await users.IsInRoleAsync((await users.FindByNameAsync("reader"))!, AdminAccess.Role));
    }
}
