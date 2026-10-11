using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.Application.Services;
public sealed class PageContentService(IRepository<PageContent> pages) : IPageContentService
{
    // The service is scoped: reuse one reviewed snapshot within a request, never across visitors.
    private IReadOnlyList<PageContentDto>? publicSnapshot;
    public async Task<IReadOnlyList<PageContentDto>> ListAsync(bool publicOnly = false, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (publicOnly && publicSnapshot is not null) { return publicSnapshot; }
        var paths = ContentPublication.Pages.Keys.ToArray();
        var rows = await pages.ListAsync(x => paths.Contains(x.Path) && (!publicOnly || (x.IsActive && x.IsReviewed)), 1, 10, ct);
        var result = rows.Where(x => ContentPublication.Pages.ContainsKey(x.Path) && (!publicOnly || ContentPublication.Complete(x.Title, x.Body))).Select(Map).ToArray();
        if (publicOnly) { publicSnapshot = Array.AsReadOnly(result); }
        return publicOnly ? publicSnapshot! : result;
    }
    public async Task<PageContentDto?> GetAsync(long id, CancellationToken ct = default)
    { var item = await pages.FindAsync(id, ct); return item is null ? null : Map(item); }
    public async Task<PageContentDto?> PublishedAsync(string path, CancellationToken ct = default)
    {
        if (!ContentPublication.Pages.ContainsKey(path))
        {
            return null;
        }

        return (await ListAsync(true, ct)).SingleOrDefault(x => x.Path == path);
    }
    public async Task SaveAsync(long? id, PageContentInput input, CancellationToken ct = default)
    {
        await new PageContentValidator().ValidateAndThrowAsync(input, ct);
        if (await pages.ExistsAsync(x => x.Path == input.Path && (!id.HasValue || x.Id != id.Value), true, ct))
        {
            throw new ConflictException("Bu sayfa zaten mevcut; mevcut kaydı düzenleyin.");
        }

        var item = id.HasValue ? await ServiceRules.RequireAsync(pages, id.Value, ct) : new PageContent();
        if (id.HasValue && item.Path != input.Path)
        {
            throw new ValidationException("Mevcut sayfanın yolu değiştirilemez.");
        }

        item.Path = input.Path; item.Title = input.Title.Trim(); item.Body = RichText.Normalize(input.Body); item.IsActive = input.IsActive; item.IsReviewed = input.IsReviewed;
        if (!id.HasValue)
        {
            pages.Add(item);
        }

        await pages.SaveAsync(ct);
        publicSnapshot = null;
    }
    public async Task DisableAsync(long id, CancellationToken ct = default)
    { var item = await ServiceRules.RequireAsync(pages, id, ct); item.IsActive = false; await pages.SaveAsync(ct); publicSnapshot = null; }
    private static PageContentDto Map(PageContent x) => new(x.Id, x.Path, x.Title, x.Body, x.IsActive, x.IsReviewed, x.UpdatedAt);
}
