using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using FluentValidation;
using CevatBiyik.Application.Validation;

namespace CevatBiyik.Application.Services;

public sealed class SettingService(IRepository<SiteSetting> settings, IValidator<UpdateSiteSettingDto> validator) : ISettingService
{
    public async Task<SiteSettingDto?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await settings.FindAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<SiteSettingDto>> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        ServiceRules.ValidatePage(page, pageSize);
        return (await settings.ListAsync(x => true, page, pageSize, cancellationToken)).Select(Map).ToArray();
    }

    public async Task<SiteSettingDto> CreateAsync(UpdateSiteSettingDto input, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        await CheckKeyAsync(input.Key, 0, cancellationToken);
        var entity = new SiteSetting { Key = input.Key, Value = input.Value.Trim(), IsActive = input.IsActive };
        settings.Add(entity);
        await settings.SaveAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<IReadOnlyList<SiteSettingDto>> ListPublicAsync(CancellationToken cancellationToken = default)
    {
        var keys = PublicSettingKeys.All.ToArray();
        var rows = await settings.ListAsync(x => x.IsActive && keys.Contains(x.Key) && x.Value != "CONTENT_REQUIRED",
            1, 100, cancellationToken);
        // A second ordinal allowlist check protects against permissive DB collation and legacy rows.
        return rows.Where(x => PublicSettingKeys.All.Contains(x.Key)).Select(Map).ToArray();
    }

    public async Task<SiteSettingDto> UpdateAsync(long id, UpdateSiteSettingDto input, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(input, cancellationToken);
        var entity = await ServiceRules.RequireAsync(settings, id, cancellationToken);
        await CheckKeyAsync(input.Key, id, cancellationToken);
        entity.Key = input.Key;
        entity.Value = input.Value.Trim();
        entity.IsActive = input.IsActive;
        await settings.SaveAsync(cancellationToken);
        return Map(entity);
    }

    public Task DeleteAsync(long id, CancellationToken cancellationToken = default) => ServiceRules.DeleteAsync(settings, id, cancellationToken);

    private async Task CheckKeyAsync(string key, long id, CancellationToken cancellationToken)
    {
        if (await settings.ExistsAsync(x => x.Key == key && x.Id != id, true, cancellationToken))
        {
            throw new ConflictException("Bu ayar anahtarı zaten kullanılıyor.");
        }
    }

    private static SiteSettingDto Map(SiteSetting x) => new(x.Id, x.Key, x.Value, x.IsActive);
}
