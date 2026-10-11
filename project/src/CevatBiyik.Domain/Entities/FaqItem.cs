namespace CevatBiyik.Domain.Entities;
public sealed class FaqItem : BaseEntity
{
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";
    public int SortOrder { get; set; }
    public bool IsReviewed { get; set; }
}
