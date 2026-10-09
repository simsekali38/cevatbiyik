namespace CevatBiyik.Domain.Entities;
public sealed class PageContent : BaseEntity
{
    public string Path { get; set; } = "";
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public bool IsReviewed { get; set; }
}
