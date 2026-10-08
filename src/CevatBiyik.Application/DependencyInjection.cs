using Microsoft.Extensions.DependencyInjection;

namespace CevatBiyik.Application;

/// <summary>
/// Application katmanı servis kaydı. Use case'ler ve doğrulayıcılar ilgili fazlarda buraya eklenir.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Faz 04+ : use case / servis kayıtları
        return services;
    }
}
