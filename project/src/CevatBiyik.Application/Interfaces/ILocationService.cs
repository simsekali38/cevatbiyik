using CevatBiyik.Application.DTOs;
namespace CevatBiyik.Application.Interfaces;
public interface ILocationService
{
    Task<IReadOnlyList<LocalLink>> ChoicesAsync(CancellationToken ct = default);
    Task<PagedResult<LocationDto>> ListAsync(int page = 1, bool publicOnly = false, CancellationToken ct = default);
    Task<LocationDto?> GetAsync(long id, CancellationToken ct = default);
    Task<LocationDto?> PublishedAsync(string slug, CancellationToken ct = default);
    Task<IReadOnlyList<LocationDto>> IndexableAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LocalLink>> RelatedAsync(long? practiceAreaId = null, long? articleId = null, CancellationToken ct = default);
    Task SaveAsync(long? id, LocationInput input, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
