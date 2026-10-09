using CevatBiyik.Application.DTOs;
namespace CevatBiyik.Application.Interfaces;
public interface ITaxonomyService
{
    Task<IReadOnlyList<TaxonomyDto>> ListAsync(bool tag, int page = 1, bool activeOnly = false, CancellationToken cancellationToken = default);
    Task<TaxonomyDto?> GetAsync(bool tag, long id, CancellationToken cancellationToken = default);
    Task<TaxonomyDto> SaveAsync(bool tag, long? id, TaxonomyInput input, CancellationToken cancellationToken = default);
    Task DeleteAsync(bool tag, long id, CancellationToken cancellationToken = default);
}
public interface IMediaService
{
    Task<MediaDto?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MediaDto>> ListAsync(int page = 1, CancellationToken cancellationToken = default);
    Task<MediaDto> UploadAsync(Stream content, string fileName, string contentType, string altText, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
public sealed record StoredImage(string StoragePath, string ContentType, int Width, int Height, long Bytes);
public interface IImageStorage
{
    Task<StoredImage> StoreAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);
}
public interface IPageSeoService
{
    Task<PageSeoDto?> ForPathAsync(string path, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PageSeoDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<PageSeoDto?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task SaveAsync(long? id, PageSeoInput input, CancellationToken cancellationToken = default);
    Task DisableAsync(long id, CancellationToken cancellationToken = default);
}
