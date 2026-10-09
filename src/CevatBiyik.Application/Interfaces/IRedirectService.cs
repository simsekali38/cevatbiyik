using CevatBiyik.Application.DTOs;
namespace CevatBiyik.Application.Interfaces;

public interface IRedirectService
{
    Task<PagedResult<RedirectDto>> ListAsync(int page = 1, CancellationToken ct = default);
    Task<RedirectDto?> GetAsync(long id, CancellationToken ct = default);
    Task SaveAsync(long? id, RedirectInput input, CancellationToken ct = default);
    Task DisableAsync(long id, CancellationToken ct = default);
    Task<string?> ResolveAsync(string path, CancellationToken ct = default);
}
public interface IPublicRouteService
{
    Task<bool> ExistsAsync(string path, CancellationToken ct = default);
}
