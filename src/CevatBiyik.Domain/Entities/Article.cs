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
}
