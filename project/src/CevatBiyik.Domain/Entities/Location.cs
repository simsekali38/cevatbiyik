namespace CevatBiyik.Domain.Entities;
public enum LocationKind { Province, District, Region }
public sealed class Location : BaseEntity
{
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public LocationKind Kind { get; set; }
    public long? ParentId { get; set; }
    public Location? Parent { get; set; }
    public string Summary { get; set; } = "";
    public string Body { get; set; } = "";
    public string LocalDetails { get; set; } = "";
    public bool IsReviewed { get; set; }
    public bool FactsVerified { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? Keywords { get; set; }
    public string? OpenGraphTitle { get; set; }
    public string? OpenGraphDescription { get; set; }
    public long? OpenGraphMediaId { get; set; }
    public MediaAsset? OpenGraphMedia { get; set; }
    public ICollection<LocationPracticeArea> PracticeLinks { get; set; } = [];
    public ICollection<LocationArticle> ArticleLinks { get; set; } = [];
}
public sealed class LocationPracticeArea : BaseEntity
{
    public long LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public long PracticeAreaId { get; set; }
    public PracticeArea PracticeArea { get; set; } = null!;
}
public sealed class LocationArticle : BaseEntity
{
    public long LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public long ArticleId { get; set; }
    public Article Article { get; set; } = null!;
}
