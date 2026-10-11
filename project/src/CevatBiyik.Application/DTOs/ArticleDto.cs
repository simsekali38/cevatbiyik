namespace CevatBiyik.Application.DTOs;

public sealed record ArticleDto(long Id, string Title, string Slug, string? Excerpt, string Content,
    DateTime? PublishedAt, long? PracticeAreaId, bool IsActive, DateTime CreatedAt, DateTime? UpdatedAt)
{
    public long? CategoryId { get; init; }
    public string? CategoryName { get; init; }
    public long[] TagIds { get; init; } = [];
    public IReadOnlyList<TagDto> Tags { get; init; } = [];
    public MediaDto? Cover { get; init; }
    public SeoInput Seo { get; init; } = new();
    public string? OpenGraphImageUrl { get; init; }
}
