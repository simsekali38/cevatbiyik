using CevatBiyik.Domain.Entities;
namespace CevatBiyik.Application.DTOs;
public sealed class LocationInput
{
    public string Name { get; init; } = "";
    public LocationKind Kind { get; init; }
    public long? ParentId { get; init; }
    public string Summary { get; init; } = "CONTENT_REQUIRED";
    public string Body { get; init; } = "CONTENT_REQUIRED";
    public string LocalDetails { get; init; } = "CONTENT_REQUIRED";
    public bool IsActive { get; init; }
    public bool IsReviewed { get; init; }
    public bool FactsVerified { get; init; }
    public SeoInput Seo { get; init; } = new();
    public long[] PracticeAreaIds { get; init; } = [];
    public long[] ArticleIds { get; init; } = [];
}
public sealed record LocalLink(long Id, string Title, string Path);
public sealed record LocationDto(long Id, string Name, string Slug, LocationInput Input, bool Indexable,
    IReadOnlyList<string> Warnings, IReadOnlyList<LocalLink> Ancestors, IReadOnlyList<LocalLink> PracticeAreas,
    IReadOnlyList<LocalLink> Articles, string? OpenGraphImageUrl, DateTime? ModifiedAt)
{
    public string Path => "/konumlar/" + Slug;
}
