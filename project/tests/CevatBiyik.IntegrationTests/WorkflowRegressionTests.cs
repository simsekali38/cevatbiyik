using System.Net;
using System.Text.RegularExpressions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Infrastructure.Email;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CevatBiyik.IntegrationTests;

/// <summary>Linked real MVC/Identity/relational workflows; not a replacement for browser or MySQL tests.</summary>
public sealed partial class WorkflowRegressionTests
{
    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex TokenPattern();
    private static async Task<HttpResponseMessage> Post(HttpClient client, string page, string target, Dictionary<string, string> values)
    {
        values["__RequestVerificationToken"] = WebUtility.HtmlDecode(TokenPattern().Match(await client.GetStringAsync(page)).Groups[1].Value);
        return await client.PostAsync(target, new FormUrlEncodedContent(values));
    }
    private static Task<HttpResponseMessage> Login(HttpClient client) => Post(client, "/admin/login", "/admin/login", new()
    { ["UserName"] = "admin", ["Password"] = AdminWebFactory.Password });

    [Fact]
    [Trait("Category", "Workflow")]
    public async Task ContactSave_QueueDelivery_AdminReadUnreadAndSoftDelete_PreserveAudit()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync();
        using var configured = factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
        { s.RemoveAll<IContactNotificationPolicy>(); s.AddSingleton<IContactNotificationPolicy>(new QueueEnabled()); }));
        using var client = configured.CreateClient(new() { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false });
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/iletisim", "/iletisim", new()
        { ["Form.Name"] = "TEST_ONLY journey", ["Form.Email"] = "test@example.test", ["Form.Message"] = "TEST_ONLY workflow message", ["Form.Consent"] = "true" })).StatusCode);
        using var scope = configured.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var row = await db.ContactMessages.SingleAsync(); Assert.True(row.NotificationPending); Assert.False(row.IsRead);
        // Exercise the durable completion path without invoking an SMTP provider.
        var store = new ContactNotificationStore(db, TimeProvider.System);
        var job = await store.ClaimAsync(default); Assert.NotNull(job);
        await store.CompleteAsync(job, true, default);
        await Login(client);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/admin")).StatusCode);
        var target = $"/admin/contact-messages/read/{row.Id}";
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync(target, new FormUrlEncodedContent(new Dictionary<string, string> { ["isRead"] = "true" }))).StatusCode);
        await db.Entry(row).ReloadAsync(); Assert.False(row.IsRead); Assert.NotNull(row.NotificationSentAt);
        foreach (var read in new[] { true, false })
        {
            Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/contact-messages", target, new() { ["isRead"] = read.ToString() })).StatusCode);
            await db.Entry(row).ReloadAsync(); Assert.Equal(read, row.IsRead);
        }
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/contact-messages", $"/admin/contact-messages/delete/{row.Id}", new() { ["confirmed"] = "true" })).StatusCode);
        db.ChangeTracker.Clear(); Assert.Empty(await db.ContactMessages.ToListAsync());
        Assert.True((await db.ContactMessages.IgnoreQueryFilters().SingleAsync()).IsDeleted);
        Assert.True(await db.AuditLogs.AnyAsync(x => x.Action == "ContactDeleted"));
        Assert.Null(await store.ClaimAsync(default));
    }

    [Fact]
    [Trait("Category", "Workflow")]
    public async Task ArticleDraft_Schedule_Publish_Delete_ChangesPublicVisibilityAndRetainsRow()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync(); using var client = factory.CreateHttpsClient();
        await Login(client);
        var input = new Dictionary<string, string> { ["Title"] = "TEST_ONLY lifecycle", ["Content"] = "TEST_ONLY content", ["IsActive"] = "false" };
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/articles/create", "/admin/articles/save", input)).StatusCode);
        using var scope = factory.Services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var row = await db.Articles.SingleAsync(); var path = "/makaleler/" + row.Slug;
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(path)).StatusCode);
        foreach (var date in new[] { "2099-01-01T12:00", "2020-01-01T12:00" })
        {
            input["IsActive"] = "true";
            input["PublishedAt"] = date;
            Assert.Equal(HttpStatusCode.Redirect, (await Post(client, $"/admin/articles/edit/{row.Id}", $"/admin/articles/save/{row.Id}", input)).StatusCode);
            Assert.Equal(date.StartsWith("2099", StringComparison.Ordinal) ? HttpStatusCode.NotFound : HttpStatusCode.OK, (await client.GetAsync(path)).StatusCode);
        }
        Assert.Contains("TEST_ONLY lifecycle", await client.GetStringAsync("/makaleler?q=lifecycle"), StringComparison.Ordinal);
        Assert.Equal(HttpStatusCode.Redirect, (await Post(client, "/admin/articles", $"/admin/articles/delete/{row.Id}", new() { ["confirmed"] = "true" })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(path)).StatusCode);
        Assert.DoesNotContain("TEST_ONLY lifecycle", await client.GetStringAsync("/makaleler?q=lifecycle"), StringComparison.Ordinal);
        db.ChangeTracker.Clear(); Assert.True((await db.Articles.IgnoreQueryFilters().SingleAsync()).IsDeleted);
        Assert.True(await db.AuditLogs.AnyAsync(x => x.Action == "ArticleDeleted"));
    }

    private sealed class QueueEnabled : IContactNotificationPolicy { public bool Enabled => true; }
}
