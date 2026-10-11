using CevatBiyik.Application.Interfaces;
using CevatBiyik.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CevatBiyik.Infrastructure.Persistence;
public sealed class FileCatalog(AppDbContext db) : IFileCatalog
{
    public Task<PrivateDocument?> DocumentAsync(long id, CancellationToken ct) => db.Set<PrivateDocument>().IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<MediaAsset?> MediaAsync(long id, CancellationToken ct) => db.MediaAssets.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IReadOnlyList<DocumentDto>> DocumentsAsync(bool deleted, int page, CancellationToken ct) =>
        await db.Set<PrivateDocument>().IgnoreQueryFilters().AsNoTracking().Where(x => x.IsDeleted == deleted)
            .OrderByDescending(x => x.Id).Skip(checked((page - 1) * 20)).Take(20).Select(x => new DocumentDto(x.Id, x.Title, x.Bytes, x.IsDeleted)).ToArrayAsync(ct);
    public async Task<IReadOnlyList<FileRecord>> RecordsAsync(bool documents, long afterId, CancellationToken ct) => documents
        ? await db.Set<PrivateDocument>().IgnoreQueryFilters().AsNoTracking().Where(x => x.Id > afterId).OrderBy(x => x.Id).Take(100).Select(x => new FileRecord(x.Id, x.StoragePath, x.IsDeleted)).ToArrayAsync(ct)
        : await db.MediaAssets.IgnoreQueryFilters().AsNoTracking().Where(x => x.Id > afterId).OrderBy(x => x.Id).Take(100).Select(x => new FileRecord(x.Id, x.StoragePath, x.IsDeleted)).ToArrayAsync(ct);
    public async Task<IReadOnlySet<string>> KnownAsync(bool documents, IReadOnlyList<string> paths, CancellationToken ct)
    {
        if (paths.Count > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(paths));
        }

        var values = paths.ToArray();
        var found = documents
            ? await db.Set<PrivateDocument>().IgnoreQueryFilters().AsNoTracking().Where(x => values.Contains(x.StoragePath)).Select(x => x.StoragePath).ToArrayAsync(ct)
            : await db.MediaAssets.IgnoreQueryFilters().AsNoTracking().Where(x => values.Contains(x.StoragePath)).Select(x => x.StoragePath).ToArrayAsync(ct);
        return found.ToHashSet(StringComparer.Ordinal);
    }
    public async Task SaveAsync(CancellationToken ct) => await db.SaveChangesAsync(ct);
}
