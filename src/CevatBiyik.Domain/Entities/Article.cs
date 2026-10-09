namespace CevatBiyik.Domain.Entities;

public sealed class Article : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public long? PracticeAreaId { get; set; }
    public PracticeArea? PracticeArea { get; set; }
    public long? CategoryId { get; set; }
    public ArticleCategory? Category { get; set; }
    public List<ArticleTagLink> TagLinks { get; set; } = [];
    public long? CoverMediaId { get; set; }
    public MediaAsset? CoverMedia { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? Keywords { get; set; }
    public string? OpenGraphTitle { get; set; }
    public string? OpenGraphDescription { get; set; }
    public long? OpenGraphMediaId { get; set; }
    public MediaAsset? OpenGraphMedia { get; set; }
}
