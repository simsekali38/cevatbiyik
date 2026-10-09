using CevatBiyik.Domain.Entities;

namespace CevatBiyik.Application.Interfaces;

public sealed record DocumentDto(long Id, string Title, long Bytes, bool IsDeleted);
public sealed record StoredDocument(string StoragePath, string Sha256, long Bytes);
public sealed record FileEntry(string Path, DateTime ModifiedAt);
public sealed record FileRecord(long Id, string Path, bool IsDeleted);
public sealed record FileReport(IReadOnlyList<FileEntry> Orphans, IReadOnlyList<FileRecord> Missing, IReadOnlyList<FileRecord> Retained, long NextId, string? NextPath, bool HasMore);

public interface IDocumentStorage
{
    Task<StoredDocument> StoreAsync(Stream content, string fileName, string contentType, CancellationToken ct = default);
    Task<Stream> OpenVerifiedAsync(string path, string sha256, CancellationToken ct = default);
    Task DeleteAsync(string path, CancellationToken ct = default);
}
public interface IMalwareScanner
{
    Task<bool> IsCleanAsync(string fullPath, CancellationToken ct = default);
}
public interface IFileCatalog
{
    Task<PrivateDocument?> DocumentAsync(long id, CancellationToken ct);
    Task<MediaAsset?> MediaAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<DocumentDto>> DocumentsAsync(bool deleted, int page, CancellationToken ct);
    Task<IReadOnlyList<FileRecord>> RecordsAsync(bool documents, long afterId, CancellationToken ct);
    Task<IReadOnlySet<string>> KnownAsync(bool documents, IReadOnlyList<string> paths, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
public interface IManagedFiles
{
    bool Exists(bool documents, string path);
    IReadOnlyList<FileEntry> List(bool documents, string? afterPath);
    Task QuarantineAsync(bool documents, string path, CancellationToken ct);
}
public interface IDocumentService
{
    Task<IReadOnlyList<DocumentDto>> ListAsync(bool deleted = false, int page = 1, CancellationToken ct = default);
    Task<DocumentDto> UploadAsync(Stream content, string fileName, string contentType, string title, CancellationToken ct = default);
    Task<Stream> DownloadAsync(long id, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
    Task RestoreAsync(long id, CancellationToken ct = default);
}
public interface IFileMaintenance
{
    Task<FileReport> ReportAsync(bool documents, long afterId = 0, string? afterPath = null, CancellationToken ct = default);
    Task QuarantineAsync(bool documents, string path, CancellationToken ct = default);
    Task RestoreImageAsync(long id, CancellationToken ct = default);
}
