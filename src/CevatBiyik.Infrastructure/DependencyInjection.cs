using CevatBiyik.Application;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CevatBiyik.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

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
        // Phase 07: file storage; Phase 11: email.
        return services;
    }
}
