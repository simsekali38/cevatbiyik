using CevatBiyik.Application.Interfaces;
using FluentValidation;
using SkiaSharp;
namespace CevatBiyik.Infrastructure.Media;
/// <summary>Only re-encoded still WebP output reaches the public uploads directory.</summary>
public sealed class FileImageStorage(string uploadsRoot) : IImageStorage, IDisposable
{
    public const int MaxBytes = 5 * 1024 * 1024;
    private readonly string root = Path.GetFullPath(uploadsRoot);
    private readonly SemaphoreSlim gate = new(2);
    public async Task<StoredImage> StoreAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var declared = extension switch { ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".webp" => "image/webp", _ => "" };
        if (declared.Length == 0 || declared != contentType)
        {
            throw new ValidationException("Yalnızca JPEG, PNG veya WebP görsel yükleyin.");
        }

        await gate.WaitAsync(cancellationToken);
        try
        {
            using var buffer = new MemoryStream();
            var chunk = new byte[81920];
            int read;
            while ((read = await content.ReadAsync(chunk, cancellationToken)) != 0)
            {
                if (buffer.Length + read > MaxBytes)
                {
                    throw new ValidationException("Görsel en fazla 5 MB olabilir.");
                }

                await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
            }
            using var data = SKData.CreateCopy(buffer.ToArray());
            using var codec = SKCodec.Create(data);
            var actual = codec?.EncodedFormat switch { SKEncodedImageFormat.Jpeg => "image/jpeg", SKEncodedImageFormat.Png => "image/png", SKEncodedImageFormat.Webp => "image/webp", _ => "" };
            if (codec is null || actual != declared || codec.FrameCount > 1 || codec.Info.Width <= 0 || codec.Info.Height <= 0
                || codec.Info.Width > 8000 || codec.Info.Height > 8000 || (long)codec.Info.Width * codec.Info.Height > 16_000_000)
            {
                throw new ValidationException("Görsel biçimi, animasyon veya piksel boyutu uygun değil.");
            }

            using var decoded = new SKBitmap(codec.Info.Width, codec.Info.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            if (codec.GetPixels(decoded.Info, decoded.GetPixels()) != SKCodecResult.Success)
            {
                throw new ValidationException("Görsel dosyası eksik veya bozuk.");
            }

            using var oriented = Orient(decoded, codec.EncodedOrigin);
            var ratio = Math.Min(1d, 1600d / Math.Max(oriented.Width, oriented.Height));
            var info = new SKImageInfo(Math.Max(1, (int)(oriented.Width * ratio)), Math.Max(1, (int)(oriented.Height * ratio)));
            using var resized = oriented.Resize(info, new SKSamplingOptions(SKFilterMode.Linear)) ?? throw new ValidationException("Görsel boyutlandırılamadı.");
            using var image = SKImage.FromBitmap(resized);
            using var encoded = image.Encode(SKEncodedImageFormat.Webp, 85);
            if (encoded is null)
            {
                throw new ValidationException("Görsel kodlanamadı.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            var path = "images/" + Guid.NewGuid().ToString("N") + ".webp";
            var full = Resolve(path);
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            var temporary = full + ".tmp";
            try
            {
                await File.WriteAllBytesAsync(temporary, encoded.ToArray(), cancellationToken);
                File.Move(temporary, full);
            }
            finally
            {
                if (File.Exists(temporary))
                {
                    File.Delete(temporary);
                }
            }
            return new(path, "image/webp", resized.Width, resized.Height, encoded.Size);
        }
        finally { gate.Release(); }
    }
    private static SKBitmap Orient(SKBitmap source, SKEncodedOrigin origin)
    {
        var swap = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var result = new SKBitmap(swap ? source.Height : source.Width, swap ? source.Width : source.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var canvas = new SKCanvas(result);
        switch (origin)
        {
            case SKEncodedOrigin.TopRight: canvas.Translate(source.Width, 0); canvas.Scale(-1, 1); break;
            case SKEncodedOrigin.BottomRight: canvas.Translate(source.Width, source.Height); canvas.RotateDegrees(180); break;
            case SKEncodedOrigin.BottomLeft: canvas.Translate(0, source.Height); canvas.Scale(1, -1); break;
            case SKEncodedOrigin.LeftTop: canvas.RotateDegrees(90); canvas.Scale(1, -1); break;
            case SKEncodedOrigin.RightTop: canvas.Translate(source.Height, 0); canvas.RotateDegrees(90); break;
            case SKEncodedOrigin.RightBottom: canvas.Translate(source.Height, source.Width); canvas.RotateDegrees(90); canvas.Scale(-1, 1); break;
            case SKEncodedOrigin.LeftBottom: canvas.Translate(0, source.Width); canvas.RotateDegrees(270); break;
        }
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }
    private string Resolve(string path)
    {
        if (path.Length != 44 || !path.StartsWith("images/", StringComparison.Ordinal) || !path.EndsWith(".webp", StringComparison.Ordinal)
            || !path.AsSpan(7, 32).ToString().All(char.IsAsciiHexDigit))
        {
            throw new ArgumentException("Invalid media path.", nameof(path));
        }

        var full = Path.GetFullPath(Path.Combine(root, path.Replace('/', Path.DirectorySeparatorChar)));
        if (!full.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid media path.", nameof(path));
        }

        return full;
    }
    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = Resolve(storagePath);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }
    public void Dispose() => gate.Dispose();
}
