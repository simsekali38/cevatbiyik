using CevatBiyik.Application.DTOs;
namespace CevatBiyik.Application.Interfaces;
public interface IProfileService
{
    Task<ProfileDto?> GetAsync(bool publicOnly = false, CancellationToken ct = default);
    Task SaveAsync(ProfileInput input, CancellationToken ct = default);
}
public interface IPageContentService
{
    Task<IReadOnlyList<PageContentDto>> ListAsync(bool publicOnly = false, CancellationToken ct = default);
    Task<PageContentDto?> GetAsync(long id, CancellationToken ct = default);
    Task<PageContentDto?> PublishedAsync(string path, CancellationToken ct = default);
    Task SaveAsync(long? id, PageContentInput input, CancellationToken ct = default);
    Task DisableAsync(long id, CancellationToken ct = default);
}
public interface IFaqService
{
    Task<PagedResult<FaqDto>> ListAsync(int page = 1, bool publicOnly = false, CancellationToken ct = default);
    Task<FaqDto?> GetAsync(long id, CancellationToken ct = default);
    Task SaveAsync(long? id, FaqInput input, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
