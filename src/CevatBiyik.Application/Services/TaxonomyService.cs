using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.Application.Services;
public sealed class TaxonomyService(IRepository<ArticleCategory> categories, IRepository<ArticleTag> tags, ISlugService slugs) : ITaxonomyService
{
    public async Task<IReadOnlyList<TaxonomyDto>> ListAsync(bool tag, int page = 1, bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        ServiceRules.ValidatePage(page, 20);
        if (tag)
        {
            return (await tags.ListAsync(x => !activeOnly || x.IsActive, page, 20, cancellationToken)).Select(Map).ToArray();
        }

        return (await categories.ListAsync(x => !activeOnly || x.IsActive, page, 20, cancellationToken)).Select(Map).ToArray();
    }
    public async Task<TaxonomyDto?> GetAsync(bool tag, long id, CancellationToken cancellationToken = default)
    {
        if (tag) { var x = await tags.FindAsync(id, cancellationToken); return x is null ? null : Map(x); }
        var category = await categories.FindAsync(id, cancellationToken); return category is null ? null : Map(category);
    }
    public async Task<TaxonomyDto> SaveAsync(bool tag, long? id, TaxonomyInput input, CancellationToken cancellationToken = default)
    {
        await new TaxonomyValidator().ValidateAndThrowAsync(input, cancellationToken);
        if (tag)
        {
            var x = id.HasValue ? await ServiceRules.RequireAsync(tags, id.Value, cancellationToken) : new ArticleTag();
            if (!id.HasValue) { x.Slug = await slugs.GenerateUniqueAsync(input.Name, (s, ct) => tags.ExistsAsync(t => t.Slug == s, true, ct), cancellationToken); tags.Add(x); }
            x.Name = input.Name.Trim(); x.IsActive = input.IsActive; await tags.SaveAsync(cancellationToken); return Map(x);
        }
        var category = id.HasValue ? await ServiceRules.RequireAsync(categories, id.Value, cancellationToken) : new ArticleCategory();
        if (!id.HasValue) { category.Slug = await slugs.GenerateUniqueAsync(input.Name, (s, ct) => categories.ExistsAsync(t => t.Slug == s, true, ct), cancellationToken); categories.Add(category); }
        category.Name = input.Name.Trim(); category.IsActive = input.IsActive; await categories.SaveAsync(cancellationToken); return Map(category);
    }
    public Task DeleteAsync(bool tag, long id, CancellationToken cancellationToken = default) =>
        tag ? ServiceRules.DeleteAsync(tags, id, cancellationToken) : ServiceRules.DeleteAsync(categories, id, cancellationToken);
    private static TaxonomyDto Map(ArticleTag x) => new(x.Id, x.Name, x.Slug, x.IsActive);
    private static TaxonomyDto Map(ArticleCategory x) => new(x.Id, x.Name, x.Slug, x.IsActive);
}
