using CevatBiyik.Application.DTOs;

namespace CevatBiyik.Application.Interfaces;

public interface IArticleService
{
    Task<ArticleDto?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArticleDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArticleDto>> ListPublishedAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<PagedResult<ArticleDto>> SearchPublishedAsync(string? search = null, long? practiceAreaId = null,
        int page = 1, int pageSize = 12, CancellationToken cancellationToken = default);
    Task<ArticleDto?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<ArticleDto> CreateAsync(CreateArticleDto input, CancellationToken cancellationToken = default);
    Task<ArticleDto> UpdateAsync(long id, UpdateArticleDto input, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
