using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;

namespace CevatBiyik.Application.Services;

public sealed class DocumentService(IRepository<PrivateDocument> repository, IFileCatalog catalog, IDocumentStorage storage) : IDocumentService
{
    public Task<IReadOnlyList<DocumentDto>> ListAsync(bool deleted = false, int page = 1, CancellationToken ct = default)
    {
        ServiceRules.ValidatePage(page, 20);
        return catalog.DocumentsAsync(deleted, page, ct);
    }
    public async Task<DocumentDto> UploadAsync(Stream content, string fileName, string contentType, string title, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length > 200 || !PlainText.IsSafe(title) || title.Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("200 karakteri aşmayan bir belge başlığı girin.");
        }

        var file = await storage.StoreAsync(content, fileName, contentType, ct);
        try
        {
            var item = new PrivateDocument { Title = title.Trim(), StoragePath = file.StoragePath, Sha256 = file.Sha256, Bytes = file.Bytes, IsActive = true };
            repository.Add(item);
            await repository.SaveAsync(ct);
            return new(item.Id, item.Title, item.Bytes, false);
        }
        catch { await storage.DeleteAsync(file.StoragePath, CancellationToken.None); throw; }
    }
    public async Task<Stream> DownloadAsync(long id, CancellationToken ct = default)
    {
        var item = await catalog.DocumentAsync(id, ct);
        if (item is not { IsDeleted: false, IsActive: true })
        {
            throw new NotFoundException("Belge bulunamadı.");
        }

        return await storage.OpenVerifiedAsync(item.StoragePath, item.Sha256, ct);
    }
    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var item = await catalog.DocumentAsync(id, ct) ?? throw new NotFoundException("Belge bulunamadı.");
        if (item.IsDeleted)
        {
            throw new NotFoundException("Belge bulunamadı.");
        }

        item.IsDeleted = true; item.IsActive = false;
        await catalog.SaveAsync(ct);
    }
    public async Task RestoreAsync(long id, CancellationToken ct = default)
    {
        var item = await catalog.DocumentAsync(id, ct) ?? throw new NotFoundException("Belge bulunamadı.");
        // Integrity and a fresh malware scan must succeed before reactivation.
        await using var verified = await storage.OpenVerifiedAsync(item.StoragePath, item.Sha256, ct);
        item.IsDeleted = false; item.IsActive = true;
        await catalog.SaveAsync(ct);
    }
}
