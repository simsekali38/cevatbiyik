namespace CevatBiyik.Application.DTOs;

/// <summary>Also used as a complete replacement input for UpdateAsync.</summary>
public sealed class CreatePracticeAreaDto
{
    public string Name { get; init; } = string.Empty;
    public string? ShortDescription { get; init; }
    public string Content { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
    public SeoInput Seo { get; init; } = new();
}
