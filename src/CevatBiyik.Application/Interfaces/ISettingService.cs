using CevatBiyik.Application.DTOs;

namespace CevatBiyik.Application.Interfaces;

public interface ISettingService
{
    Task<SiteSettingDto?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SiteSettingDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SiteSettingDto>> ListPublicAsync(CancellationToken cancellationToken = default);
    Task<SiteSettingDto> CreateAsync(UpdateSiteSettingDto input, CancellationToken cancellationToken = default);
    Task<SiteSettingDto> UpdateAsync(long id, UpdateSiteSettingDto input, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
