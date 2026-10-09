namespace CevatBiyik.Domain.Entities;
public sealed class MediaAsset : BaseEntity
{
    public string StoragePath { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public long Bytes { get; set; }
}
