using CevatBiyik.Application.Interfaces;
using FluentValidation;
using SkiaSharp;

namespace CevatBiyik.Infrastructure.Media;
public sealed class ManagedFiles(string imageRoot, string privateRoot, TimeProvider clock) : IManagedFiles
{
    private string Root(bool documents) => documents ? privateRoot : imageRoot;
    private string Resolve(bool documents, string path)
    {
        var temporary = documents ? ".pending" : ".tmp";
        if (!path.EndsWith(temporary, StringComparison.Ordinal))
        {
            return SafeFilePaths.Resolve(Root(documents), documents, path);
        }

        var full = SafeFilePaths.Resolve(Root(documents), documents, path[..^temporary.Length]) + temporary;
        SafeFilePaths.NoLinks(full);
        return full;
    }
    public bool Exists(bool documents, string path)
    {
        var full = SafeFilePaths.Resolve(Root(documents), documents, path);
        if (!File.Exists(full))
        {
            return false;
        }

        if (documents)
        {
            return true;
        }

        var length = new FileInfo(full).Length;
        if (length <= 0 || length > FileImageStorage.MaxBytes)
        {
            return false;
        }

        using var data = SKData.Create(full);
        using var codec = SKCodec.Create(data);
        if (codec is null || codec.EncodedFormat != SKEncodedImageFormat.Webp || codec.FrameCount > 1
            || codec.Info.Width <= 0 || codec.Info.Height <= 0 || codec.Info.Width > 1600 || codec.Info.Height > 1600)
        {
            return false;
        }

        using var bitmap = new SKBitmap(codec.Info.Width, codec.Info.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        return codec.GetPixels(bitmap.Info, bitmap.GetPixels()) == SKCodecResult.Success;
    }
    public IReadOnlyList<FileEntry> List(bool documents, string? afterPath)
    {
        var root = Root(documents);
        var folder = Path.Combine(root, documents ? "documents" : "images");
        SafeFilePaths.NoLinks(folder);
        if (!Directory.Exists(folder))
        {
            return Array.Empty<FileEntry>();
        }
        // Bounded memory, lexicographic continuation; includes only managed final files.
        var selected = new SortedDictionary<string, FileEntry>(StringComparer.Ordinal);
        foreach (var full in Directory.EnumerateFiles(folder))
        {
            var key = (documents ? "documents/" : "images/") + Path.GetFileName(full);
            try { Resolve(documents, key); } catch (ValidationException) { continue; }
            if (afterPath is not null && string.CompareOrdinal(key, afterPath) <= 0)
            {
                continue;
            }

            selected[key] = new(key, File.GetLastWriteTimeUtc(full));
            if (selected.Count > 100)
            {
                selected.Remove(selected.Keys.Last());
            }
        }
        return selected.Values.ToArray();
    }
    public Task QuarantineAsync(bool documents, string path, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var full = Resolve(documents, path);
        if (!File.Exists(full) || File.GetLastWriteTimeUtc(full) > clock.GetUtcNow().UtcDateTime.AddHours(-48))
        {
            throw new ValidationException("Yalnızca en az 48 saatlik sahipsiz dosyalar taşınabilir.");
        }

        var folder = Path.Combine(privateRoot, "quarantine", documents ? "documents" : "images");
        SafeFilePaths.NoLinks(folder); Directory.CreateDirectory(folder); SafeFilePaths.NoLinks(folder);
        // Filesystem mutation cannot be rolled back by a DB transaction. Never overwrite; retain the key for operator recovery.
        File.Move(full, Path.Combine(folder, Path.GetFileName(full)), false);
        return Task.CompletedTask;
    }
}
