namespace CevatBiyik.Application.DTOs;

public sealed record PracticeAreaDto(long Id, string Name, string Slug, string? ShortDescription,
    string Content, int SortOrder, bool IsActive);
