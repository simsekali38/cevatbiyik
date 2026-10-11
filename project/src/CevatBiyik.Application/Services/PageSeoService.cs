using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.Application.Services;
public sealed class PageSeoService(IRepository<PageSeo> pages, IRepository<MediaAsset> media) : IPageSeoService
{
    public async Task<PageSeoDto?> ForPathAsync(string path, CancellationToken cancellationToken = default)
    {
        var rows = await pages.ListAsync(x => x.Path == path && x.IsActive, 1, 1, cancellationToken);
        return rows.Count == 0 ? null : Map(rows[0]);
    }
    public async Task<IReadOnlyList<PageSeoDto>> ListAsync(CancellationToken cancellationToken = default) =>
        (await pages.ListAsync(x => true, 1, 20, cancellationToken)).Select(Map).ToArray();
    public async Task<PageSeoDto?> GetAsync(long id, CancellationToken cancellationToken = default)
    { var x = await pages.FindAsync(id, cancellationToken); return x is null ? null : Map(x); }
    public async Task SaveAsync(long? id, PageSeoInput input, CancellationToken cancellationToken = default)
    {
        await new PageSeoValidator().ValidateAndThrowAsync(input, cancellationToken);
        if (await pages.ExistsAsync(x => x.Path == input.Path && (!id.HasValue || x.Id != id), true, cancellationToken))
        {
            throw new ConflictException("Bu sayfa için SEO kaydı zaten mevcut.");
        }

        if (input.Seo.OpenGraphMediaId is { } image && !await media.ExistsAsync(x => x.Id == image && x.IsActive, cancellationToken: cancellationToken))
        {
            throw new NotFoundException("Görsel bulunamadı.");
        }

        var x = id.HasValue ? await ServiceRules.RequireAsync(pages, id.Value, cancellationToken) : new PageSeo();
        x.Path = input.Path; x.MetaTitle = input.Seo.MetaTitle; x.MetaDescription = input.Seo.MetaDescription; x.Keywords = input.Seo.Keywords;
        x.OpenGraphTitle = input.Seo.OpenGraphTitle; x.OpenGraphDescription = input.Seo.OpenGraphDescription; x.OpenGraphMediaId = input.Seo.OpenGraphMediaId;
        x.IsActive = input.IsActive; if (!id.HasValue)
        {
            pages.Add(x);
        }

        await pages.SaveAsync(cancellationToken);
    }
    public async Task DisableAsync(long id, CancellationToken cancellationToken = default)
    {
        var page = await ServiceRules.RequireAsync(pages, id, cancellationToken);
        page.IsActive = false;
        await pages.SaveAsync(cancellationToken);
    }
    private static PageSeoDto Map(PageSeo x) => new(x.Id, x.Path, new SeoInput
    {
        MetaTitle = x.MetaTitle,
        MetaDescription = x.MetaDescription,
        Keywords = x.Keywords,
        OpenGraphTitle = x.OpenGraphTitle,
        OpenGraphDescription = x.OpenGraphDescription,
        OpenGraphMediaId = x.OpenGraphMediaId
    },
        x.OpenGraphMedia is { IsActive: true, IsDeleted: false } m ? "/uploads/" + m.StoragePath : null, x.IsActive);
}
