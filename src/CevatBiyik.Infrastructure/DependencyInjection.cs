using CevatBiyik.Application;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Infrastructure.Persistence;
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
        // Faz 03 : Identity
        // Faz 05 : dosya depolama
        // Faz 09 : e-posta
        return services;
    }
}
