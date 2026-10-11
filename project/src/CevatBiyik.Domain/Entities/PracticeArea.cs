namespace CevatBiyik.Domain.Entities;

public sealed class PracticeArea : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string Content { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? Keywords { get; set; }
    public string? OpenGraphTitle { get; set; }
    public string? OpenGraphDescription { get; set; }
    public long? OpenGraphMediaId { get; set; }
    public MediaAsset? OpenGraphMedia { get; set; }
    public ICollection<Article> Articles { get; } = new List<Article>();
}
