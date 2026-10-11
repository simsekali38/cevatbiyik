using CevatBiyik.Infrastructure.Media;
using FluentValidation;
using SkiaSharp;
namespace CevatBiyik.IntegrationTests;
public sealed class ImageStorageTests
{
    public static byte[] Png(int width = 2000, int height = 1000)
    {
        using var bitmap = new SKBitmap(width, height); bitmap.Erase(SKColors.Navy);
        using var image = SKImage.FromBitmap(bitmap); using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
    [Fact]
    public async Task Reencoding_BoundsDimensions_RemovesTrailingPayload_AndNeverUsesUserFilename()
    {
        var root = Path.Combine(Path.GetTempPath(), "cevat-image-test-" + Guid.NewGuid().ToString("N"));
        using var storage = new FileImageStorage(root);
        var original = Png().Concat("<script>PAYLOAD</script>"u8.ToArray()).ToArray();
        using var stream = new MemoryStream(original);
        var stored = await storage.StoreAsync(stream, "../../user-name.png", "image/png");
        try
        {
            Assert.Equal(1600, stored.Width); Assert.Equal(800, stored.Height);
            Assert.Equal("image/webp", stored.ContentType);
            Assert.DoesNotContain("user-name", stored.StoragePath, StringComparison.Ordinal);
            var bytes = await File.ReadAllBytesAsync(Path.Combine(root, stored.StoragePath));
            Assert.DoesNotContain("PAYLOAD", System.Text.Encoding.UTF8.GetString(bytes), StringComparison.Ordinal);
            using var data = SKData.CreateCopy(bytes); using var codec = SKCodec.Create(data);
            Assert.Equal(SKEncodedImageFormat.Webp, codec.EncodedFormat); Assert.Equal(1600, codec.Info.Width);
            await Assert.ThrowsAsync<ArgumentException>(() => storage.DeleteAsync("../secret.txt"));
        }
        finally
        {
            await storage.DeleteAsync(stored.StoragePath); Directory.Delete(Path.Combine(root, "images")); Directory.Delete(root);
        }
    }
    [Theory]
    [InlineData("attack.svg", "image/svg+xml")]
    [InlineData("attack.aspx", "image/png")]
    [InlineData("test.png", "image/jpeg")]
    [InlineData("test.jpg", "image/jpeg")]
    public async Task UnsafeExtensionsMimeMismatchAndCorruptFilesAreRejected(string name, string mime)
    {
        var root = Path.Combine(Path.GetTempPath(), "cevat-image-test-" + Guid.NewGuid().ToString("N"));
        using var storage = new FileImageStorage(root); using var stream = new MemoryStream("<svg onload=alert(1)>"u8.ToArray());
        await Assert.ThrowsAsync<ValidationException>(() => storage.StoreAsync(stream, name, mime));
        Assert.False(Directory.Exists(root));
    }
    [Fact]
    public async Task ByteAndPixelLimitsRejectBeforeFilesystemWrites()
    {
        var root = Path.Combine(Path.GetTempPath(), "cevat-image-test-" + Guid.NewGuid().ToString("N"));
        using var storage = new FileImageStorage(root);
        using var oversized = new MemoryStream(new byte[FileImageStorage.MaxBytes + 1]);
        await Assert.ThrowsAsync<ValidationException>(() => storage.StoreAsync(oversized, "test.png", "image/png"));
        using var pixels = new MemoryStream(Png(8001, 1));
        await Assert.ThrowsAsync<ValidationException>(() => storage.StoreAsync(pixels, "test.png", "image/png"));
        using var truncated = new MemoryStream(Png()[..60]);
        await Assert.ThrowsAsync<ValidationException>(() => storage.StoreAsync(truncated, "test.png", "image/png"));
        Assert.False(Directory.Exists(root));
    }
}
