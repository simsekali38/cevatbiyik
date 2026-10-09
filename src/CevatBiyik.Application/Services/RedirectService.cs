using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.Application.Services;

public sealed class RedirectService(IRepository<RedirectRule> rules, IPublicRouteService routes) : IRedirectService
{
    public async Task<PagedResult<RedirectDto>> ListAsync(int page = 1, CancellationToken ct = default)
    {
        ServiceRules.ValidatePage(page, 20);
        return new((await rules.ListAsync(x => true, page, 20, ct)).Select(Map).ToArray(), page, 20, await rules.CountAsync(x => true, ct));
    }
    public async Task<RedirectDto?> GetAsync(long id, CancellationToken ct = default)
    { var item = await rules.FindAsync(id, ct); return item is null ? null : Map(item); }
    public async Task SaveAsync(long? id, RedirectInput input, CancellationToken ct = default)
    {
        await new RedirectValidator().ValidateAndThrowAsync(input, ct);
        var source = RedirectPaths.Normalize(input.SourcePath); var target = RedirectPaths.Normalize(input.DestinationPath);
        if (await rules.ExistsAsync(x => x.SourcePath == source && (!id.HasValue || x.Id != id.Value), true, ct))
        {
            throw new ConflictException("Bu kaynak için mevcut yönlendirmeyi düzenleyin.");
        }

        if (await routes.ExistsAsync(source, ct))
        {
            throw new ValidationException("Yayımlanmış bir rotanın üzerine yönlendirme eklenemez.");
        }

        if (!await routes.ExistsAsync(target, ct))
        {
            throw new ValidationException("Hedef, mevcut ve yayımlanmış bir public sayfa olmalıdır.");
        }

        if (await rules.ExistsAsync(x => x.SourcePath == target && x.IsActive && (!id.HasValue || x.Id != id.Value), cancellationToken: ct))
        {
            throw new ValidationException("Yönlendirme zinciri oluşturulamaz; doğrudan son sayfayı seçin.");
        }

        var item = id.HasValue ? await ServiceRules.RequireAsync(rules, id.Value, ct) : new RedirectRule();
        if (id.HasValue && item.SourcePath != source)
        {
            throw new ValidationException("Mevcut kaynak yolu değiştirilemez; yeni kural ekleyin.");
        }

        item.SourcePath = source; item.DestinationPath = target; item.IsActive = input.IsActive;
        if (!id.HasValue)
        {
            rules.Add(item);
        }

        await rules.SaveAsync(ct);
    }
    public async Task DisableAsync(long id, CancellationToken ct = default)
    { var item = await ServiceRules.RequireAsync(rules, id, ct); item.IsActive = false; await rules.SaveAsync(ct); }
    public async Task<string?> ResolveAsync(string path, CancellationToken ct = default)
    {
        if (!RedirectPaths.IsSource(path))
        {
            return null;
        }

        var source = RedirectPaths.Normalize(path);
        var rows = await rules.ListAsync(x => x.SourcePath == source && x.IsActive, 1, 1, ct);
        if (rows.Count == 0)
        {
            return null;
        }

        var target = rows[0].DestinationPath;
        // Revalidate stored data and publication at request time; current content always wins.
        if (!RedirectPaths.IsSafe(target) || source == target || await routes.ExistsAsync(source, ct) || !await routes.ExistsAsync(target, ct))
        {
            return null;
        }

        return target;
    }
    private static RedirectDto Map(RedirectRule item) => new(item.Id, item.SourcePath, item.DestinationPath, item.IsActive);
}
