namespace CevatBiyik.Domain.Entities;

public sealed class PracticeArea : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string Content { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public ICollection<Article> Articles { get; } = new List<Article>();
}
