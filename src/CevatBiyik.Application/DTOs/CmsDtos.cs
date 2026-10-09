namespace CevatBiyik.Application.DTOs;
public sealed record TaxonomyDto(long Id, string Name, string Slug, bool IsActive);
public sealed class TaxonomyInput
{
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}
public sealed record TagDto(long Id, string Name, string Slug);
public sealed record MediaDto(long Id, string StoragePath, string AltText, string ContentType, int Width, int Height, long Bytes)
{
    public string Url => "/uploads/" + StoragePath;
}
public sealed class SeoInput
{
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
    public string? Keywords { get; init; }
    public string? OpenGraphTitle { get; init; }
    public string? OpenGraphDescription { get; init; }
    public long? OpenGraphMediaId { get; init; }
}
public sealed class PageSeoInput
{
    public string Path { get; init; } = string.Empty;
    public SeoInput Seo { get; init; } = new();
    public bool IsActive { get; init; }
}
public sealed record PageSeoDto(long Id, string Path, SeoInput Seo, string? OpenGraphImageUrl, bool IsActive);
