namespace CevatBiyik.Domain.Entities;
public sealed class ArticleTag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
