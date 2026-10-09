using CevatBiyik.Application.Interfaces;
using CevatBiyik.Infrastructure.Identity;
using CevatBiyik.Infrastructure.Media;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CevatBiyik.IntegrationTests;

/// <summary>Real Identity, cookies, Application services and relational repositories; isolated SQLite, never MySQL.</summary>
public sealed class AdminWebFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    public const string Password = "Test-Only_Strong9!";
    public string UploadRoot { get; } = Path.Combine(Path.GetTempPath(), "cevat-cms-tests-" + Guid.NewGuid().ToString("N"));
    public bool AllowIndexing { get; init; }
    public bool LegalServiceSchemaEnabled { get; init; }
    public bool FailMediaAudit { get; init; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        connection.Open();
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=127.0.0.1;Database=unused;User ID=unused;",
            ["Site:AllowIndexing"] = AllowIndexing ? "true" : "false",
            ["Site:LegalServiceSchemaEnabled"] = LegalServiceSchemaEnabled ? "true" : "false"
        }));
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
            services.RemoveAll<IImageStorage>();
            services.AddSingleton<IImageStorage>(_ => new FileImageStorage(UploadRoot));
            if (FailMediaAudit)
            {
                services.RemoveAll<IAuditLogService>();
                services.AddScoped<IAuditLogService, FailingMediaAudit>();
            }
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        Assert.True((await roles.CreateAsync(new IdentityRole(AdminAccess.Role))).Succeeded);
        foreach (var name in new[] { "admin", "reader", "disabled", "locked", "twofactor" })
        {
            var user = new ApplicationUser
            {
                UserName = name,
                Email = name + "@example.test",
                EmailConfirmed = true,
                IsActive = name != "disabled",
                TwoFactorEnabled = name == "twofactor"
            };
            Assert.True((await users.CreateAsync(user, Password)).Succeeded);
            if (name != "reader")
            {
                Assert.True((await users.AddToRoleAsync(user, AdminAccess.Role)).Succeeded);
            }
        }
    }

    public HttpClient CreateHttpsClient() => CreateClient(new WebApplicationFactoryClientOptions
    { BaseAddress = new Uri("https://localhost"), AllowAutoRedirect = false, HandleCookies = true });

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            connection.Dispose();
            if (Directory.Exists(UploadRoot))
            {
                foreach (var file in Directory.GetFiles(Path.Combine(UploadRoot, "images")))
                {
                    File.Delete(file);
                }

                Directory.Delete(Path.Combine(UploadRoot, "images"));
                Directory.Delete(UploadRoot);
            }
        }
    }
}

internal sealed class FailingMediaAudit(AppDbContext db) : IAuditLogService
{
    public Task WriteAsync(SecurityEvent securityEvent, string? actorId, CancellationToken cancellationToken = default) =>
        securityEvent == SecurityEvent.MediaUploaded ? throw new InvalidOperationException("Test-only audit failure.")
        : new AuditLogService(db).WriteAsync(securityEvent, actorId, cancellationToken);
    public Task<IReadOnlyList<AuditLogDto>> RecentAsync(CancellationToken cancellationToken = default) => new AuditLogService(db).RecentAsync(cancellationToken);
}
