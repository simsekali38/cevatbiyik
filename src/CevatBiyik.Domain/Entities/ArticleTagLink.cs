namespace CevatBiyik.Domain.Entities;
public sealed class ArticleTagLink
{
    public long ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public long TagId { get; set; }
    public ArticleTag Tag { get; set; } = null!;
}
