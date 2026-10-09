using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.Application.Services;
public sealed class MediaService(IRepository<MediaAsset> media, IRepository<Article> articles, IRepository<PageSeo> pages, IImageStorage storage, IRepository<PracticeArea> areas) : IMediaService
{
    public async Task<MediaDto?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var item = await media.FindAsync(id, cancellationToken);
        return item is { IsActive: true } ? Map(item) : null;
    }
    public async Task<IReadOnlyList<MediaDto>> ListAsync(int page = 1, CancellationToken cancellationToken = default)
    {
        ServiceRules.ValidatePage(page, 20);
        return (await media.ListAsync(x => x.IsActive, page, 20, cancellationToken)).Select(Map).ToArray();
    }
    public async Task<MediaDto> UploadAsync(Stream content, string fileName, string contentType, string altText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(altText) || altText.Length > 200 || !PlainText.IsSafe(altText) || altText.Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("Görsel için doğrulanmış, 200 karakteri aşmayan alternatif metin girin.");
        }

        var image = await storage.StoreAsync(content, fileName, contentType, cancellationToken);
        try
        {
            var x = new MediaAsset
            {
                StoragePath = image.StoragePath,
                AltText = altText.Trim(),
                ContentType = image.ContentType,
                Width = image.Width,
                Height = image.Height,
                Bytes = image.Bytes,
                IsActive = true
            };
            media.Add(x); await media.SaveAsync(cancellationToken); return Map(x);
        }
        catch { await storage.DeleteAsync(image.StoragePath, CancellationToken.None); throw; }
    }
    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        if (await articles.ExistsAsync(x => x.CoverMediaId == id || x.OpenGraphMediaId == id, true, cancellationToken)
            || await pages.ExistsAsync(x => x.OpenGraphMediaId == id, true, cancellationToken)
            || await areas.ExistsAsync(x => x.OpenGraphMediaId == id, true, cancellationToken))
        {
            throw new ConflictException("Bu görsel içerikte kullanılıyor; önce içerikteki bağlantıyı kaldırın.");
        }

        await ServiceRules.DeleteAsync(media, id, cancellationToken);
    }
    public static MediaDto Map(MediaAsset x) => new(x.Id, x.StoragePath, x.AltText, x.ContentType, x.Width, x.Height, x.Bytes);
}
