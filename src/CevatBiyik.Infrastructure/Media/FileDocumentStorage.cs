using System.Security.Cryptography;
using CevatBiyik.Application.Interfaces;
using FluentValidation;

namespace CevatBiyik.Infrastructure.Media;
public sealed class FileDocumentStorage : IDocumentStorage
{
    private readonly string root;
    private readonly IMalwareScanner scanner;
    public FileDocumentStorage(string privateRoot, string publicRoot, IMalwareScanner scanner)
    {
        root = Path.GetFullPath(privateRoot);
        var web = Path.GetFullPath(publicRoot).TrimEnd(Path.DirectorySeparatorChar);
        if (root.Equals(web, StringComparison.OrdinalIgnoreCase) || root.StartsWith(web + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Private documents must be outside wwwroot.");
        }

        SafeFilePaths.NoLinks(root); this.scanner = scanner;
    }
    public async Task<StoredDocument> StoreAsync(Stream content, string fileName, string contentType, CancellationToken ct = default)
    {
        if (SafeFilePaths.Extension(fileName) != ".pdf" || contentType != "application/pdf")
        {
            throw new ValidationException("Yalnızca PDF belgeleri kabul edilir.");
        }

        var bytes = await SafeFilePaths.ReadBoundedAsync(content, ct);
        PdfPolicy.Validate(bytes);
        var key = "documents/" + Guid.NewGuid().ToString("N") + ".pdf";
        var full = SafeFilePaths.Resolve(root, true, key);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        SafeFilePaths.NoLinks(full);
        var temporary = full + ".pending";
        try
        {
            SafeFilePaths.NoLinks(temporary);
            await File.WriteAllBytesAsync(temporary, bytes, ct);
            SafeFilePaths.NoLinks(temporary);
            if (!await scanner.IsCleanAsync(temporary, ct))
            {
                throw new ValidationException("Antivirüs doğrulaması başarısız; belge kabul edilmedi.");
            }
            // Ensure the scanned file has not been changed by another process.
            SafeFilePaths.NoLinks(temporary);
            await using (var scannedInput = new FileStream(temporary, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true))
            {
                var scanned = await SafeFilePaths.ReadBoundedAsync(scannedInput, ct);
                if (!CryptographicOperations.FixedTimeEquals(SHA256.HashData(bytes), SHA256.HashData(scanned)))
                {
                    throw new ValidationException("Belge bütünlüğü doğrulanamadı.");
                }
            }
            File.Move(temporary, full);
            return new(key, Convert.ToHexString(SHA256.HashData(bytes)), bytes.LongLength);
        }
        finally { if (File.Exists(temporary))
            {
                File.Delete(temporary);
            }
        }
    }
    public async Task<Stream> OpenVerifiedAsync(string path, string sha256, CancellationToken ct = default)
    {
        var full = SafeFilePaths.Resolve(root, true, path);
        if (!File.Exists(full))
        {
            throw new ValidationException("Belge dosyası eksik; önce yedekten kurtarın.");
        }

        await using var input = new FileStream(full, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
        var bytes = await SafeFilePaths.ReadBoundedAsync(input, ct);
        if (!string.Equals(Convert.ToHexString(SHA256.HashData(bytes)), sha256, StringComparison.Ordinal))
        {
            throw new ValidationException("Belge bütünlüğü doğrulanamadı.");
        }

        PdfPolicy.Validate(bytes);
        if (!await scanner.IsCleanAsync(full, ct))
        {
            throw new ValidationException("Antivirüs doğrulaması başarısız; belge indirilemiyor.");
        }

        return new MemoryStream(bytes, false);
    }
    public Task DeleteAsync(string path, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested(); File.Delete(SafeFilePaths.Resolve(root, true, path)); return Task.CompletedTask;
    }
}
