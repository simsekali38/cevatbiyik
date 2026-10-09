namespace CevatBiyik.Application.DTOs;

public sealed record ArticleDto(long Id, string Title, string Slug, string? Excerpt, string Content,
    DateTime? PublishedAt, long? PracticeAreaId, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt);
