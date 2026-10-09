using CevatBiyik.Application;
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

        // Faz 02 : DbContext (MySQL 8)
        // Faz 03 : Identity
        // Faz 05 : dosya depolama
        // Faz 09 : e-posta
        return services;
    }
}
