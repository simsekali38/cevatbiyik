using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.IntegrationTests;
public sealed partial class DocumentFlowTests
{
    private static readonly byte[] Pdf = CevatBiyik.Tests.PdfFixture.Create();
    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex TokenPattern();
    private static async Task<string> Token(HttpClient client, string path) => WebUtility.HtmlDecode(TokenPattern().Match(await client.GetStringAsync(path)).Groups[1].Value);
    private static async Task Login(HttpClient client, string name = "admin")
    {
        var token = await Token(client, "/admin/login");
        using var response = await client.PostAsync("/admin/login", new FormUrlEncodedContent(new Dictionary<string, string>
        { ["UserName"] = name, ["Password"] = AdminWebFactory.Password, ["__RequestVerificationToken"] = token }));
        Assert.Equal(name == "admin" ? HttpStatusCode.Redirect : HttpStatusCode.OK, response.StatusCode);
    }
    private static async Task<HttpResponseMessage> Upload(HttpClient client, string name = "document.pdf", string mime = "application/pdf", bool token = true)
    {
        using var form = new MultipartFormDataContent();
        if (token)
        {
            form.Add(new StringContent(await Token(client, "/admin/documents")), "__RequestVerificationToken");
        }

        form.Add(new StringContent("Test belge"), "title");
        var file = new ByteArrayContent(Pdf); file.Headers.ContentType = new MediaTypeHeaderValue(mime); form.Add(file, "file", name);
        return await client.PostAsync("/admin/documents/upload", form);
    }
    [Fact]
    public async Task Documents_RequireAdminAndCsrf_AndNeverExposePrivateFiles()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync(); using var client = factory.CreateHttpsClient();
        foreach (var path in new[] { "/admin/documents", "/admin/documents/download/1", "/admin/files" })
        {
            Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync(path)).StatusCode);
        }

        await Login(client, "reader");
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/admin/documents/download/1")).StatusCode);
        using var admin = factory.CreateHttpsClient(); await Login(admin);
        Assert.Equal(HttpStatusCode.BadRequest, (await Upload(admin, token: false)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync("/App_Data/private-files/documents/test.pdf")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await admin.GetAsync("/uploads/documents/test.pdf")).StatusCode);
    }
    [Fact]
    public async Task UploadDownloadDeleteRestore_AreAuditedAndIntegrityChecked()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync(); using var client = factory.CreateHttpsClient(); await Login(client);
        Assert.Equal(HttpStatusCode.Redirect, (await Upload(client)).StatusCode);
        string storagePath;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var item = await db.PrivateDocuments.SingleAsync(); storagePath = item.StoragePath;
            Assert.Equal(64, item.Sha256.Length);
            Assert.Contains(await db.AuditLogs.ToArrayAsync(), x => x.Action == nameof(SecurityEvent.DocumentUploaded));
        }
        using var download = await client.GetAsync("/admin/documents/download/1");
        Assert.Equal(HttpStatusCode.OK, download.StatusCode);
        Assert.Equal("attachment", download.Content.Headers.ContentDisposition!.DispositionType);
        Assert.Equal("application/pdf", download.Content.Headers.ContentType!.MediaType);
        Assert.True(download.Headers.CacheControl!.NoStore);
        Assert.Equal(Pdf, await download.Content.ReadAsByteArrayAsync());
        foreach (var action in new[] { "delete", "restore" })
        {
            var token = await Token(client, "/admin/documents?deleted=true");
            using var response = await client.PostAsync($"/admin/documents/{action}/1", new FormUrlEncodedContent(new Dictionary<string, string> { ["confirmed"] = "true", ["__RequestVerificationToken"] = token }));
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            if (action == "delete")
            {
                Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/admin/documents/download/1")).StatusCode);
            }
        }
        await File.AppendAllTextAsync(Path.Combine(factory.PrivateRoot, storagePath.Replace('/', Path.DirectorySeparatorChar)), "tampered");
        Assert.Equal(HttpStatusCode.Conflict, (await client.GetAsync("/admin/documents/download/1")).StatusCode);
        // Restore the original file bytes, as an isolated recovery smoke test.
        await File.WriteAllBytesAsync(Path.Combine(factory.PrivateRoot, storagePath.Replace('/', Path.DirectorySeparatorChar)), Pdf);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/admin/documents/download/1")).StatusCode);
    }
    [Theory]
    [InlineData("bad.exe", "application/pdf")]
    [InlineData("bad.pdf", "image/png")]
    public async Task InvalidUploads_DoNotCreateDatabaseRows(string name, string mime)
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync(); using var client = factory.CreateHttpsClient(); await Login(client);
        Assert.Equal(HttpStatusCode.Redirect, (await Upload(client, name, mime)).StatusCode);
        using var scope = factory.Services.CreateScope();
        Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().PrivateDocuments.ToArrayAsync());
    }
    [Fact]
    public async Task FailedAudit_RollsBackDocumentAndCompensatesFilesystem()
    {
        using var factory = new AdminWebFactory { FailDocumentAudit = true }; await factory.InitializeAsync(); using var client = factory.CreateHttpsClient(); await Login(client);
        Assert.Equal(HttpStatusCode.InternalServerError, (await Upload(client)).StatusCode);
        using var scope = factory.Services.CreateScope();
        Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().PrivateDocuments.ToArrayAsync());
        Assert.Empty(Directory.GetFiles(Path.Combine(factory.PrivateRoot, "documents")));
    }
    [Fact]
    public async Task FailedMalwareScan_LeavesNeitherStoredDocumentNorDatabaseRecord()
    {
        using var factory = new AdminWebFactory { RejectDocumentScan = true }; await factory.InitializeAsync(); using var client = factory.CreateHttpsClient(); await Login(client);
        Assert.Equal(HttpStatusCode.Redirect, (await Upload(client)).StatusCode);
        using var scope = factory.Services.CreateScope();
        Assert.Empty(await scope.ServiceProvider.GetRequiredService<AppDbContext>().PrivateDocuments.ToArrayAsync());
        Assert.Empty(Directory.GetFiles(Path.Combine(factory.PrivateRoot, "documents")));
    }
    [Fact]
    public async Task Reconciliation_IncludesDeletedRows_AndReportsMissingAndOrphanFiles()
    {
        using var factory = new AdminWebFactory(); await factory.InitializeAsync(); using var client = factory.CreateHttpsClient(); await Login(client);
        await Upload(client);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var item = await db.PrivateDocuments.SingleAsync(); item.IsDeleted = true; await db.SaveChangesAsync();
        var maintenance = scope.ServiceProvider.GetRequiredService<IFileMaintenance>();
        Assert.Empty((await maintenance.ReportAsync(true)).Orphans);
        File.Delete(Path.Combine(factory.PrivateRoot, item.StoragePath.Replace('/', Path.DirectorySeparatorChar)));
        var orphan = "documents/" + new string('a', 32) + ".pdf";
        await File.WriteAllBytesAsync(Path.Combine(factory.PrivateRoot, orphan.Replace('/', Path.DirectorySeparatorChar)), Pdf);
        var report = await maintenance.ReportAsync(true);
        Assert.Single(report.Orphans); Assert.Single(report.Missing);
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => maintenance.QuarantineAsync(true, orphan));
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/admin/files?documents=true")).StatusCode);
    }
}
