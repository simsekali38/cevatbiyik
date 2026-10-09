using CevatBiyik.Application.DTOs;

namespace CevatBiyik.Application.Interfaces;

public interface IPracticeAreaService
{
    Task<PracticeAreaDto?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PracticeAreaDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<PagedResult<PracticeAreaDto>> ListActiveAsync(int page = 1, int pageSize = 12, CancellationToken cancellationToken = default);
    Task<PracticeAreaDto?> GetActiveBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<PracticeAreaDto> CreateAsync(CreatePracticeAreaDto input, CancellationToken cancellationToken = default);
    Task<PracticeAreaDto> UpdateAsync(long id, CreatePracticeAreaDto input, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
