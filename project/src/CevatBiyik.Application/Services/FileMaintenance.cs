using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using FluentValidation;

namespace CevatBiyik.Application.Services;

public sealed class FileMaintenance(IFileCatalog catalog, IManagedFiles files) : IFileMaintenance
{
    public async Task<FileReport> ReportAsync(bool documents, long afterId = 0, string? afterPath = null, CancellationToken ct = default)
    {
        if (afterId < 0 || afterPath?.Length > 100)
        {
            throw new ValidationException("Geçersiz mutabakat sayfası.");
        }

        var records = await catalog.RecordsAsync(documents, afterId, ct);
        var batch = files.List(documents, afterPath);
        var known = await catalog.KnownAsync(documents, batch.Select(x => x.Path).ToArray(), ct);
        var missing = records.Where(x => !files.Exists(documents, x.Path)).ToArray();
        return new(batch.Where(x => !known.Contains(x.Path)).ToArray(), missing, records.Where(x => x.IsDeleted && !missing.Contains(x)).ToArray(),
            records.Count > 0 ? records[^1].Id : afterId, batch.Count > 0 ? batch[^1].Path : afterPath, records.Count == 100 || batch.Count == 100);
    }
    public async Task QuarantineAsync(bool documents, string path, CancellationToken ct = default)
    {
        if ((await catalog.KnownAsync(documents, new[] { path }, ct)).Count != 0)
        {
            throw new ValidationException("Veritabanına bağlı dosya karantinaya taşınamaz.");
        }
        // Age is rechecked by storage immediately before the atomic move.
        await files.QuarantineAsync(documents, path, ct);
    }
    public async Task RestoreImageAsync(long id, CancellationToken ct = default)
    {
        var item = await catalog.MediaAsync(id, ct) ?? throw new NotFoundException("Görsel bulunamadı.");
        if (!files.Exists(false, item.StoragePath))
        {
            throw new ValidationException("Görsel dosyası eksik; önce yedekten kurtarın.");
        }

        item.IsDeleted = false; item.IsActive = true;
        await catalog.SaveAsync(ct);
    }
}
