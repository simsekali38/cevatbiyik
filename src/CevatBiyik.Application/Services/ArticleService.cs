using System.Linq.Expressions;
using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using FluentValidation;

namespace CevatBiyik.Application.Services;

public sealed class ArticleService(IRepository<Article> articles, IRepository<PracticeArea> areas, ISlugService slugs,
    IValidator<CreateArticleDto> createValidator, IValidator<UpdateArticleDto> updateValidator, TimeProvider timeProvider) : IArticleService
{
    public async Task<ArticleDto?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await articles.FindAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public Task<IReadOnlyList<ArticleDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default) =>
        ListCoreAsync(x => true, page, pageSize, cancellationToken);

    public Task<IReadOnlyList<ArticleDto>> ListPublishedAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        return ListCoreAsync(x => x.IsActive && x.PublishedAt != null && x.PublishedAt <= now &&
            (x.PracticeAreaId == null || (x.PracticeArea != null && x.PracticeArea.IsActive && !x.PracticeArea.IsDeleted)),
            page, pageSize, cancellationToken);
    }

    public async Task<PagedResult<ArticleDto>> SearchPublishedAsync(string? search = null, long? practiceAreaId = null,
        int page = 1, int pageSize = 12, CancellationToken cancellationToken = default)
    {
        ServiceRules.ValidatePage(page, pageSize);
        if (search?.Length > 100 || practiceAreaId is <= 0)
        {
            throw new ArgumentException("Geçersiz arama veya çalışma alanı filtresi.");
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var term = search?.Trim() ?? string.Empty;
        Expression<Func<Article, bool>> predicate = x => x.IsActive && x.PublishedAt != null && x.PublishedAt <= now &&
            (x.PracticeAreaId == null || (x.PracticeArea != null && x.PracticeArea.IsActive && !x.PracticeArea.IsDeleted)) &&
            (!practiceAreaId.HasValue || x.PracticeAreaId == practiceAreaId) &&
            (term == string.Empty || x.Title.Contains(term) || (x.Excerpt != null && x.Excerpt.Contains(term)));
        var count = await articles.CountAsync(predicate, cancellationToken);
        var rows = await articles.ListAsync(predicate, page, pageSize,
            x => x.PublishedAt!, true, cancellationToken);
        return new(rows.Select(Map).ToArray(), page, pageSize, count);
    }

    public async Task<ArticleDto?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(slug);
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var rows = await articles.ListAsync(x => x.Slug == slug && x.IsActive && x.PublishedAt != null && x.PublishedAt <= now &&
            (x.PracticeAreaId == null || (x.PracticeArea != null && x.PracticeArea.IsActive && !x.PracticeArea.IsDeleted)),
            1, 1, cancellationToken);
        return rows.Count == 0 ? null : Map(rows[0]);
    }

    private async Task<IReadOnlyList<ArticleDto>> ListCoreAsync(Expression<Func<Article, bool>> predicate,
        int page, int pageSize, CancellationToken cancellationToken)
    {
        ServiceRules.ValidatePage(page, pageSize);
        return (await articles.ListAsync(predicate, page, pageSize, cancellationToken)).Select(Map).ToArray();
    }

    public async Task<ArticleDto> CreateAsync(CreateArticleDto input, CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(input, cancellationToken);
        await ValidateAreaAsync(input.PracticeAreaId, cancellationToken);
        var entity = new Article
        {
            Slug = await slugs.GenerateUniqueAsync(input.Title,
                (candidate, ct) => articles.ExistsAsync(x => x.Slug == candidate, true, ct), cancellationToken)
        };
        Apply(entity, input);
        articles.Add(entity);
        await articles.SaveAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<ArticleDto> UpdateAsync(long id, UpdateArticleDto input, CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(input, cancellationToken);
        await ValidateAreaAsync(input.PracticeAreaId, cancellationToken);
        var entity = await ServiceRules.RequireAsync(articles, id, cancellationToken);
        Apply(entity, input);
        await articles.SaveAsync(cancellationToken);
        return Map(entity);
    }

    public Task DeleteAsync(long id, CancellationToken cancellationToken = default) => ServiceRules.DeleteAsync(articles, id, cancellationToken);

    private async Task ValidateAreaAsync(long? id, CancellationToken cancellationToken)
    {
        if (id.HasValue && !await areas.ExistsAsync(x => x.Id == id.Value, cancellationToken: cancellationToken))
        {
            throw new NotFoundException("Çalışma alanı bulunamadı.");
        }
    }

    private static void Apply(Article entity, CreateArticleDto input)
    {
        entity.Title = input.Title.Trim();
        entity.Excerpt = input.Excerpt?.Trim();
        entity.Content = input.Content.Trim();
        entity.PublishedAt = input.PublishedAt;
        entity.PracticeAreaId = input.PracticeAreaId;
        entity.IsActive = input.IsActive;
    }

    private static ArticleDto Map(Article x) => new(x.Id, x.Title, x.Slug, x.Excerpt, x.Content,
        ServiceRules.AsUtc(x.PublishedAt), x.PracticeAreaId, x.IsActive, ServiceRules.AsUtc(x.CreatedAt), ServiceRules.AsUtc(x.UpdatedAt));
}
