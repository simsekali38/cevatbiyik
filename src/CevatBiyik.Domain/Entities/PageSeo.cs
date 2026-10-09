namespace CevatBiyik.Domain.Entities;
public sealed class PageSeo : BaseEntity
{
    public string Path { get; set; } = string.Empty;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? Keywords { get; set; }
    public string? OpenGraphTitle { get; set; }
    public string? OpenGraphDescription { get; set; }
    public long? OpenGraphMediaId { get; set; }
    public MediaAsset? OpenGraphMedia { get; set; }
}
