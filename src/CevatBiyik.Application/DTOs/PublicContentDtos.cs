namespace CevatBiyik.Application.DTOs;
public sealed class ProfileInput
{
    public string Biography { get; set; } = "CONTENT_REQUIRED";
    public string Vision { get; set; } = "CONTENT_REQUIRED";
    public string BarAssociation { get; set; } = "CONTENT_REQUIRED";
    public bool IsActive { get; set; }
    public bool IsReviewed { get; set; }
}
public sealed class PageContentInput
{
    public string Path { get; set; } = "/kvkk";
    public string Title { get; set; } = "CONTENT_REQUIRED";
    public string Body { get; set; } = "CONTENT_REQUIRED";
    public bool IsActive { get; set; }
    public bool IsReviewed { get; set; }
}
public sealed class FaqInput
{
    public string Question { get; set; } = "CONTENT_REQUIRED";
    public string Answer { get; set; } = "CONTENT_REQUIRED";
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsReviewed { get; set; }
}
public sealed record ProfileDto(long Id, string Biography, string Vision, string BarAssociation, bool IsActive, bool IsReviewed);
public sealed record PageContentDto(long Id, string Path, string Title, string Body, bool IsActive, bool IsReviewed, DateTime? UpdatedAt);
public sealed record FaqDto(long Id, string Question, string Answer, int SortOrder, bool IsActive, bool IsReviewed);
