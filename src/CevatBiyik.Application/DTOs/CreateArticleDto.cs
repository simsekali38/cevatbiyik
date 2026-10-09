namespace CevatBiyik.Application.DTOs;

public class CreateArticleDto
{
    public string Title { get; init; } = string.Empty;
    public string? Excerpt { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime? PublishedAt { get; init; }
    public long? PracticeAreaId { get; init; }
    public bool IsActive { get; init; }
}
