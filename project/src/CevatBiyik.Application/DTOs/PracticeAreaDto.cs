namespace CevatBiyik.Application.DTOs;

public sealed record PracticeAreaDto(long Id, string Name, string Slug, string? ShortDescription,
    string Content, int SortOrder, bool IsActive)
{
    public SeoInput Seo { get; init; } = new();
    public string? OpenGraphImageUrl { get; init; }
}
