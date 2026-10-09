using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using FluentValidation;

namespace CevatBiyik.Application.Services;

public sealed class PracticeAreaService(IRepository<PracticeArea> areas, ISlugService slugs,
    IValidator<CreatePracticeAreaDto> validator, IRepository<MediaAsset> media) : IPracticeAreaService
{
    public async Task<PracticeAreaDto?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await areas.FindAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<PracticeAreaDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        ServiceRules.ValidatePage(page, pageSize);
        return (await areas.ListAsync(x => true, page, pageSize, cancellationToken)).Select(Map).ToArray();
    }

    public async Task<PracticeAreaDto> CreateAsync(CreatePracticeAreaDto input, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        await ValidateMediaAsync(input, cancellationToken);
        var entity = new PracticeArea
        {
            Slug = await slugs.GenerateUniqueAsync(input.Name,
                (candidate, ct) => areas.ExistsAsync(x => x.Slug == candidate, true, ct), cancellationToken)
        };
        Apply(entity, input);
        areas.Add(entity);
        await areas.SaveAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<PagedResult<PracticeAreaDto>> ListActiveAsync(int page = 1, int pageSize = 12, CancellationToken cancellationToken = default)
    {
        ServiceRules.ValidatePage(page, pageSize);
        var count = await areas.CountAsync(x => x.IsActive, cancellationToken);
        var rows = await areas.ListAsync(x => x.IsActive, page, pageSize,
            x => x.SortOrder, false, cancellationToken);
        return new(rows.Select(Map).ToArray(), page, pageSize, count);
    }

    public async Task<PracticeAreaDto?> GetActiveBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var rows = await areas.ListAsync(x => x.Slug == slug && x.IsActive, 1, 1, cancellationToken);
        return rows.Count == 0 ? null : Map(rows[0]);
    }

    public async Task<PracticeAreaDto> UpdateAsync(long id, CreatePracticeAreaDto input, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        await ValidateMediaAsync(input, cancellationToken);
        var entity = await ServiceRules.RequireAsync(areas, id, cancellationToken);
        Apply(entity, input);
        await areas.SaveAsync(cancellationToken);
        return Map(entity);
    }

    public Task DeleteAsync(long id, CancellationToken cancellationToken = default) => ServiceRules.DeleteAsync(areas, id, cancellationToken);

    private static void Apply(PracticeArea entity, CreatePracticeAreaDto input)
    {
        entity.Name = input.Name.Trim();
        entity.ShortDescription = input.ShortDescription?.Trim();
        entity.Content = input.Content.Trim();
        entity.SortOrder = input.SortOrder;
        entity.IsActive = input.IsActive;
        entity.MetaTitle = input.Seo.MetaTitle; entity.MetaDescription = input.Seo.MetaDescription; entity.Keywords = input.Seo.Keywords;
        entity.OpenGraphTitle = input.Seo.OpenGraphTitle; entity.OpenGraphDescription = input.Seo.OpenGraphDescription; entity.OpenGraphMediaId = input.Seo.OpenGraphMediaId;
    }

    private async Task ValidateMediaAsync(CreatePracticeAreaDto input, CancellationToken ct)
    {
        if (input.Seo.OpenGraphMediaId is { } id && !await media.ExistsAsync(x => x.Id == id && x.IsActive, cancellationToken: ct))
        {
            throw new CevatBiyik.Application.Exceptions.NotFoundException("Görsel bulunamadı.");
        }
    }
    private static PracticeAreaDto Map(PracticeArea x) => new(x.Id, x.Name, x.Slug, x.ShortDescription, x.Content, x.SortOrder, x.IsActive)
    {
        Seo = new SeoInput { MetaTitle = x.MetaTitle, MetaDescription = x.MetaDescription, Keywords = x.Keywords,
            OpenGraphTitle = x.OpenGraphTitle, OpenGraphDescription = x.OpenGraphDescription, OpenGraphMediaId = x.OpenGraphMediaId },
        OpenGraphImageUrl = x.OpenGraphMedia is { IsActive: true, IsDeleted: false } m ? "/uploads/" + m.StoragePath : null
    };
}
