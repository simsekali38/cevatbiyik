using CevatBiyik.Application;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Infrastructure.Identity;
using CevatBiyik.Infrastructure.Media;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.Infrastructure;

/// <summary>
/// Infrastructure katmanı servis kaydı. Bağlantı dizesi ve diğer sırlar kaynak kodda tutulmaz;
/// IConfiguration üzerinden (user-secrets / ortam değişkeni) okunur.
/// Application kayıtları da buradan çağrılır (Web yalnızca Infrastructure'a referans verir).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddApplication();
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 12;
            options.Password.RequiredUniqueChars = 4;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.User.RequireUniqueEmail = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.AllowedForNewUsers = true;
        }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
        services.AddScoped<IAdminAuthenticationService, AdminAuthenticationService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IAdminOperation, AdminOperation>();
        services.AddScoped<ISitemapService, SitemapService>();
        services.AddScoped<IFileCatalog, FileCatalog>();
        services.AddSingleton<IMalwareScanner>(_ => new DefenderScanner(configuration["Storage:DefenderExecutable"]));
        services.AddSingleton<IDocumentStorage>(provider =>
        {
            var environment = provider.GetRequiredService<IWebHostEnvironment>();
            var web = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
            var privateRoot = configuration["Storage:PrivateRoot"] ?? Path.Combine(environment.ContentRootPath, "App_Data", "private-files");
            return new FileDocumentStorage(privateRoot, web, provider.GetRequiredService<IMalwareScanner>());
        });
        services.AddSingleton<IManagedFiles>(provider =>
        {
            var environment = provider.GetRequiredService<IWebHostEnvironment>();
            var web = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
            var privateRoot = configuration["Storage:PrivateRoot"] ?? Path.Combine(environment.ContentRootPath, "App_Data", "private-files");
            // Validate the private-root boundary even when only maintenance is resolved.
            _ = new FileDocumentStorage(privateRoot, web, provider.GetRequiredService<IMalwareScanner>());
            return new ManagedFiles(Path.Combine(web, "uploads"), privateRoot, provider.GetRequiredService<TimeProvider>());
        });
        services.AddSingleton<IImageStorage>(provider =>
        {
            var environment = provider.GetRequiredService<IWebHostEnvironment>();
            return new FileImageStorage(Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "uploads"));
        });
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("ConnectionStrings:DefaultConnection must be configured via user-secrets or environment variables.");
            }

            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
        });
        var smtp = Email.SmtpNotificationOptions.Read(configuration);
        smtp.Validate();
        services.AddSingleton(smtp);
        services.AddSingleton<IContactNotificationPolicy>(smtp);
        services.AddScoped<IContactEmailSender, Email.SmtpContactEmailSender>();
        services.AddScoped<IContactNotificationStore, Email.ContactNotificationStore>();
        services.AddHostedService<Email.ContactNotificationWorker>();
        return services;
    }
}
