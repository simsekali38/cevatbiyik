using FluentValidation;

namespace CevatBiyik.Infrastructure.Media;
public static class SafeFilePaths
{
    public static string Extension(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 255 || name.Any(char.IsControl) || name.Contains(':') || name.EndsWith('.') || name.EndsWith(' '))
        {
            throw new ValidationException("Geçersiz dosya adı.");
        }
        // Browser-supplied names are never used on disk, including directory components.
        return Path.GetExtension(name.Replace('\\', '/').Split('/')[^1]).ToLowerInvariant();
    }
    public static void NoLinks(string path)
    {
        for (var current = Path.GetFullPath(path); current is not null; current = Path.GetDirectoryName(current))
        {
            if ((File.Exists(current) || Directory.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
            {
                throw new ValidationException("Dosya deposunda bağlantı veya junction kullanılamaz.");
            }
        }
    }
    public static string Resolve(string root, bool documents, string key)
    {
        var prefix = documents ? "documents/" : "images/";
        var suffix = documents ? ".pdf" : ".webp";
        if (key.Length != prefix.Length + 32 + suffix.Length || !key.StartsWith(prefix, StringComparison.Ordinal) || !key.EndsWith(suffix, StringComparison.Ordinal)
            || !key.AsSpan(prefix.Length, 32).ToString().All(char.IsAsciiHexDigit))
        {
            throw new ValidationException("Geçersiz dosya anahtarı.");
        }

        var full = Path.GetFullPath(Path.Combine(root, key.Replace('/', Path.DirectorySeparatorChar)));
        NoLinks(full);
        return full;
    }
    public static async Task<byte[]> ReadBoundedAsync(Stream content, CancellationToken ct)
    {
        using var memory = new MemoryStream();
        var chunk = new byte[81920];
        int count;
        while ((count = await content.ReadAsync(chunk, ct)) != 0)
        {
            if (memory.Length + count > FileImageStorage.MaxBytes)
            {
                throw new ValidationException("Dosya en fazla 5 MiB olabilir.");
            }

            await memory.WriteAsync(chunk.AsMemory(0, count), ct);
        }
        if (memory.Length == 0)
        {
            throw new ValidationException("Boş dosya yüklenemez.");
        }

        return memory.ToArray();
    }
}
