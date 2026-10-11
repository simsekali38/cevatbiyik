using System.Linq.Expressions;
using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;

namespace CevatBiyik.Application.Services;

public sealed class ArticleService(IRepository<Article> articles, IRepository<PracticeArea> areas, ISlugService slugs,
    IValidator<CreateArticleDto> createValidator, IValidator<UpdateArticleDto> updateValidator, TimeProvider timeProvider,
    IRepository<ArticleCategory>? categories = null, IRepository<ArticleTag>? tags = null, IRepository<MediaAsset>? media = null) : IArticleService
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

    public async Task<PagedResult<ArticleDto>> SearchCmsPublishedAsync(string? search, long? practiceAreaId, long? categoryId, string? tag,
        int page = 1, CancellationToken cancellationToken = default)
    {
        ServiceRules.ValidatePage(page, 12);
        if (search?.Length > 100 || practiceAreaId is <= 0 || categoryId is <= 0 || tag?.Length > 200)
        {
            throw new ArgumentException("Geçersiz makale filtresi.");
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var term = search?.Trim() ?? string.Empty;
        Expression<Func<Article, bool>> predicate = x => x.IsActive && x.PublishedAt != null && x.PublishedAt <= now
            && (x.PracticeAreaId == null || (x.PracticeArea != null && x.PracticeArea.IsActive && !x.PracticeArea.IsDeleted))
            && (!practiceAreaId.HasValue || x.PracticeAreaId == practiceAreaId)
            && (!categoryId.HasValue || (x.CategoryId == categoryId && x.Category != null && x.Category.IsActive && !x.Category.IsDeleted))
            && (tag == null || x.TagLinks.Any(t => t.Tag.Slug == tag && t.Tag.IsActive && !t.Tag.IsDeleted))
            && (term == string.Empty || x.Title.Contains(term) || (x.Excerpt != null && x.Excerpt.Contains(term)));
        var count = await articles.CountAsync(predicate, cancellationToken);
        var rows = await articles.ListAsync(predicate, page, 12, x => x.PublishedAt!, true, cancellationToken);
        return new(rows.Select(Map).ToArray(), page, 12, count);
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
        var assignedTags = await ValidateCmsAsync(input, cancellationToken);
        var entity = new Article
        {
            Slug = await slugs.GenerateUniqueAsync(input.Title,
                (candidate, ct) => articles.ExistsAsync(x => x.Slug == candidate, true, ct), cancellationToken)
        };
        Apply(entity, input);
        ApplyRelations(entity, assignedTags);
        entity.TagLinks = assignedTags.Tags.Select(x => new ArticleTagLink { TagId = x.Id, Tag = x, Article = entity }).ToList();
        articles.Add(entity);
        await articles.SaveAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<ArticleDto> UpdateAsync(long id, UpdateArticleDto input, CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(input, cancellationToken);
        await ValidateAreaAsync(input.PracticeAreaId, cancellationToken);
        var assignedTags = await ValidateCmsAsync(input, cancellationToken);
        var entity = await ServiceRules.RequireAsync(articles, id, cancellationToken);
        Apply(entity, input);
        ApplyRelations(entity, assignedTags);
        var ids = assignedTags.Tags.Select(x => x.Id).ToHashSet();
        entity.TagLinks.RemoveAll(x => !ids.Contains(x.TagId));
        foreach (var tag in assignedTags.Tags.Where(x => entity.TagLinks.All(link => link.TagId != x.Id)))
        {
            entity.TagLinks.Add(new ArticleTagLink { Article = entity, TagId = tag.Id, Tag = tag });
        }

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
        entity.Content = RichText.Normalize(input.Content);
        entity.PublishedAt = input.PublishedAt;
        entity.PracticeAreaId = input.PracticeAreaId;
        entity.IsActive = input.IsActive;
        entity.CategoryId = input.CategoryId;
        entity.CoverMediaId = input.CoverMediaId;
        entity.MetaTitle = input.Seo.MetaTitle; entity.MetaDescription = input.Seo.MetaDescription;
        entity.Keywords = input.Seo.Keywords; entity.OpenGraphTitle = input.Seo.OpenGraphTitle;
        entity.OpenGraphDescription = input.Seo.OpenGraphDescription; entity.OpenGraphMediaId = input.Seo.OpenGraphMediaId;
    }

    private sealed record CmsRelations(ArticleCategory? Category, IReadOnlyList<ArticleTag> Tags, MediaAsset? Cover, MediaAsset? OpenGraph);
    private static void ApplyRelations(Article entity, CmsRelations relations)
    {
        entity.Category = relations.Category;
        entity.CoverMedia = relations.Cover;
        entity.OpenGraphMedia = relations.OpenGraph;
    }
    private async Task<CmsRelations> ValidateCmsAsync(CreateArticleDto input, CancellationToken cancellationToken)
    {
        await new CevatBiyik.Application.Validation.SeoValidator().ValidateAndThrowAsync(input.Seo, cancellationToken);
        if (input.TagIds is null || input.TagIds.Length > 20 || input.TagIds.Any(x => x <= 0) || input.TagIds.Distinct().Count() != input.TagIds.Length)
        {
            throw new ValidationException("En fazla 20 farklı etiket seçin.");
        }

        var category = input.CategoryId is { } categoryId && categories is not null ? await categories.FindAsync(categoryId, cancellationToken) : null;
        if (input.CategoryId.HasValue && category is not { IsActive: true })
        {
            throw new NotFoundException("Makale kategorisi bulunamadı veya etkin değil.");
        }

        var images = new Dictionary<long, MediaAsset>();
        foreach (var id in new[] { input.CoverMediaId, input.Seo.OpenGraphMediaId }.Where(x => x.HasValue).Distinct())
        {
            var image = media is null ? null : await media.FindAsync(id!.Value, cancellationToken);
            if (image is not { IsActive: true })
            {
                throw new NotFoundException("Görsel bulunamadı.");
            }
            images.Add(image.Id, image);
        }

        var result = new List<ArticleTag>();
        foreach (var id in input.TagIds)
        {
            var tag = tags is null ? null : await tags.FindAsync(id, cancellationToken);
            if (tag is null || !tag.IsActive)
            {
                throw new NotFoundException("Etiket bulunamadı veya etkin değil.");
            }

            result.Add(tag);
        }
        return new(category, result, input.CoverMediaId is { } cover ? images[cover] : null,
            input.Seo.OpenGraphMediaId is { } og ? images[og] : null);
    }

    private static ArticleDto Map(Article x) => new(x.Id, x.Title, x.Slug, x.Excerpt, x.Content,
        ServiceRules.AsUtc(x.PublishedAt), x.PracticeAreaId, x.IsActive, ServiceRules.AsUtc(x.CreatedAt), ServiceRules.AsUtc(x.UpdatedAt))
    {
        CategoryId = x.CategoryId,
        CategoryName = x.Category is { IsActive: true, IsDeleted: false } c ? c.Name : null,
        TagIds = x.TagLinks.Select(t => t.TagId).ToArray(),
        Tags = x.TagLinks.Where(t => t.Tag is { IsActive: true, IsDeleted: false }).Select(t => new TagDto(t.Tag.Id, t.Tag.Name, t.Tag.Slug)).ToArray(),
        Cover = x.CoverMedia is { IsActive: true, IsDeleted: false } m ? MediaService.Map(m) : null,
        Seo = new SeoInput
        {
            MetaTitle = x.MetaTitle,
            MetaDescription = x.MetaDescription,
            Keywords = x.Keywords,
            OpenGraphTitle = x.OpenGraphTitle,
            OpenGraphDescription = x.OpenGraphDescription,
            OpenGraphMediaId = x.OpenGraphMediaId
        },
        OpenGraphImageUrl = x.OpenGraphMedia is { IsActive: true, IsDeleted: false } image ? "/uploads/" + image.StoragePath : null
    };
}
