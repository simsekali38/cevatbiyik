using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.Application.Services;
public sealed class LocationService(IRepository<Location> locations, IRepository<PracticeArea> areas,
    IRepository<Article> articles, IRepository<MediaAsset> media, ISlugService slugs, TimeProvider clock) : ILocationService
{
    public const int MaximumLocations = 100;
    private async Task<IReadOnlyList<Location>> AllAsync(CancellationToken ct)
    {
        if (await locations.CountAsync(x => true, ct) > MaximumLocations)
        {
            throw new ConflictException("Konum kapsamı 100 kaydı aşıyor; mimari inceleme gerekli.");
        }
        return await locations.ListAsync(x => true, 1, MaximumLocations, x => x.Id, false, ct);
    }
    public async Task<IReadOnlyList<LocalLink>> ChoicesAsync(CancellationToken ct = default) =>
        (await AllAsync(ct)).Select(x => new LocalLink(x.Id, x.Name + " (" + (x.Kind == LocationKind.Province ? "İl" : x.Kind == LocationKind.District ? "İlçe" : "Bölge") + ")", "/konumlar/" + x.Slug)).ToArray();
    public async Task<PagedResult<LocationDto>> ListAsync(int page = 1, bool publicOnly = false, CancellationToken ct = default)
    {
        ServiceRules.ValidatePage(page, 12);
        var all = await AllAsync(ct); var quality = LocalContentQuality.CheckAll(all); var mapped = all.Select(x => Map(x, all, quality)).Where(x => !publicOnly || x.Indexable).ToArray();
        return new(mapped.Skip((page - 1) * 12).Take(12).ToArray(), page, 12, mapped.Length);
    }
    public async Task<LocationDto?> GetAsync(long id, CancellationToken ct = default)
    { var all = await AllAsync(ct); var item = all.FirstOrDefault(x => x.Id == id); return item is null ? null : Map(item, all, LocalContentQuality.CheckAll(all)); }
    public async Task<LocationDto?> PublishedAsync(string slug, CancellationToken ct = default)
    { var all = await AllAsync(ct); var item = all.FirstOrDefault(x => x.Slug == slug && x.IsActive && x.IsReviewed); return item is null ? null : Map(item, all, LocalContentQuality.CheckAll(all)); }
    public async Task<IReadOnlyList<LocationDto>> IndexableAsync(CancellationToken ct = default)
    { var all = await AllAsync(ct); var quality = LocalContentQuality.CheckAll(all); return all.Select(x => Map(x, all, quality)).Where(x => x.Indexable).ToArray(); }
    public async Task<IReadOnlyList<LocalLink>> RelatedAsync(long? practiceAreaId = null, long? articleId = null, CancellationToken ct = default) =>
        (await IndexableAsync(ct)).Where(x => (practiceAreaId.HasValue && x.PracticeAreas.Any(a => a.Id == practiceAreaId)) ||
            (articleId.HasValue && x.Articles.Any(a => a.Id == articleId))).Select(x => new LocalLink(x.Id, x.Name, x.Path)).ToArray();
    public async Task SaveAsync(long? id, LocationInput input, CancellationToken ct = default)
    {
        await new LocationValidator().ValidateAndThrowAsync(input, ct);
        var all = await AllAsync(ct);
        if (!id.HasValue && all.Count >= MaximumLocations)
        {
            throw new ValidationException("En fazla 100 konum; otomatik bölge çoğaltımı desteklenmez.");
        }
        var item = id.HasValue ? await ServiceRules.RequireAsync(locations, id.Value, ct) : new Location();
        if (id.HasValue && (item.Kind != input.Kind || item.ParentId != input.ParentId))
        {
            throw new ValidationException("Hiyerarşi ve konum türü oluşturulduktan sonra değiştirilemez.");
        }
        Location? parent = null;
        if (input.Kind != LocationKind.Province)
        {
            parent = all.FirstOrDefault(x => x.Id == input.ParentId);
            if (parent is null || parent.Id == id || (input.Kind == LocationKind.District && parent.Kind != LocationKind.Province) ||
                (input.Kind == LocationKind.Region && parent.Kind != LocationKind.District) || !ValidHierarchy(parent, all))
            {
                throw new ValidationException("İlçe Kayseri iline, bölge geçerli Kayseri ilçesine bağlanmalıdır.");
            }
        }
        var slug = id.HasValue ? item.Slug : slugs.Generate(input.Name);
        if (await locations.ExistsAsync(x => x.Slug == slug && (!id.HasValue || x.Id != id.Value), true, ct))
        {
            throw new ConflictException("Bu konum slug değeri ayrılmış; mevcut kaydı düzenleyin.");
        }
        var selectedAreas = await areas.ListAsync(x => input.PracticeAreaIds.Contains(x.Id) && x.IsActive, 1, 20, ct);
        var now = clock.GetUtcNow().UtcDateTime;
        var selectedArticles = await articles.ListAsync(x => input.ArticleIds.Contains(x.Id) && x.IsActive && x.PublishedAt != null && x.PublishedAt <= now && (x.PracticeAreaId == null || (x.PracticeArea != null && x.PracticeArea.IsActive && !x.PracticeArea.IsDeleted)), 1, 20, ct);
        if (selectedAreas.Count != input.PracticeAreaIds.Length || selectedArticles.Count != input.ArticleIds.Length)
        {
            throw new ValidationException("Yalnızca mevcut aktif alanlar ve yayımlanmış makaleler bağlanabilir.");
        }
        if (input.Seo.OpenGraphMediaId is { } mediaId && (await media.FindAsync(mediaId, ct)) is not { IsActive: true })
        {
            throw new ValidationException("OpenGraph görseli aktif medya olmalıdır.");
        }
        item.Name = input.Name.Trim(); item.Slug = slug; item.Kind = input.Kind; item.ParentId = input.ParentId;
        item.Summary = input.Summary.Trim(); item.Body = input.Body.Trim(); item.LocalDetails = input.LocalDetails.Trim();
        item.IsActive = input.IsActive; item.IsReviewed = input.IsReviewed; item.FactsVerified = input.FactsVerified;
        item.MetaTitle = input.Seo.MetaTitle; item.MetaDescription = input.Seo.MetaDescription; item.Keywords = input.Seo.Keywords;
        item.OpenGraphTitle = input.Seo.OpenGraphTitle; item.OpenGraphDescription = input.Seo.OpenGraphDescription; item.OpenGraphMediaId = input.Seo.OpenGraphMediaId;
        foreach (var link in item.PracticeLinks.Where(x => !input.PracticeAreaIds.Contains(x.PracticeAreaId)).ToArray())
        {
            item.PracticeLinks.Remove(link);
        }
        foreach (var area in selectedAreas.Where(x => !item.PracticeLinks.Any(l => l.PracticeAreaId == x.Id)))
        {
            item.PracticeLinks.Add(new LocationPracticeArea { PracticeAreaId = area.Id, IsActive = true });
        }
        foreach (var link in item.ArticleLinks.Where(x => !input.ArticleIds.Contains(x.ArticleId)).ToArray())
        {
            item.ArticleLinks.Remove(link);
        }
        foreach (var article in selectedArticles.Where(x => !item.ArticleLinks.Any(l => l.ArticleId == x.Id)))
        {
            item.ArticleLinks.Add(new LocationArticle { ArticleId = article.Id, IsActive = true });
        }
        if (!id.HasValue)
        {
            locations.Add(item);
        }
        await locations.SaveAsync(ct);
    }
    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        if (await locations.ExistsAsync(x => x.ParentId == id, cancellationToken: ct))
        {
            throw new ConflictException("Alt konumlar varken silinemez; önce alt kayıtları kaldırın.");
        }
        await ServiceRules.DeleteAsync(locations, id, ct);
    }
    private static bool ValidHierarchy(Location x, IReadOnlyList<Location> all) => x.Kind switch
    {
        LocationKind.Province => x.Name == "Kayseri" && x.ParentId is null,
        LocationKind.District => all.Any(p => p.Id == x.ParentId && p.Kind == LocationKind.Province && p.Name == "Kayseri" && p.ParentId is null),
        LocationKind.Region => all.Any(p => p.Id == x.ParentId && p.Kind == LocationKind.District && ValidHierarchy(p, all)),
        _ => false
    };
    private LocationDto Map(Location item, IReadOnlyList<Location> all, IReadOnlyDictionary<long, IReadOnlyList<string>> quality)
    {
        var warnings = quality[item.Id].ToList();
        if (!ValidHierarchy(item, all))
        {
            warnings.Add("Kayseri hiyerarşisi geçersiz.");
        }
        var ancestors = new List<LocalLink>(); var parentId = item.ParentId;
        for (var depth = 0; parentId.HasValue && depth < 2; depth++)
        {
            var parent = all.FirstOrDefault(x => x.Id == parentId);
            if (parent is null || !ValidHierarchy(parent, all) || !Map(parent, all, quality).Indexable)
            {
                warnings.Add("Üst konum yayımlanabilir ve indekslenebilir olmalıdır."); break;
            }
            ancestors.Insert(0, new(parent.Id, parent.Name, "/konumlar/" + parent.Slug)); parentId = parent.ParentId;
        }
        var now = clock.GetUtcNow().UtcDateTime;
        var areaLinks = item.PracticeLinks.Where(x => !x.IsDeleted && x.IsActive && x.PracticeArea is { IsActive: true, IsDeleted: false })
            .Take(20).Select(x => new LocalLink(x.PracticeAreaId, x.PracticeArea.Name, "/uzmanlik-alanlari/" + x.PracticeArea.Slug)).ToArray();
        var articleLinks = item.ArticleLinks.Where(x => !x.IsDeleted && x.IsActive && x.Article is { IsActive: true, IsDeleted: false, PublishedAt: not null } && x.Article.PublishedAt <= now && (x.Article.PracticeAreaId == null || x.Article.PracticeArea is { IsActive: true, IsDeleted: false }))
            .Take(20).Select(x => new LocalLink(x.ArticleId, x.Article.Title, "/makaleler/" + x.Article.Slug)).ToArray();
        if (areaLinks.Length + articleLinks.Length == 0)
        {
            warnings.Add("En az bir ilgili yayımlanmış alan veya makale bağlantısı gerekir.");
        }
        var seo = new SeoInput { MetaTitle = item.MetaTitle, MetaDescription = item.MetaDescription, Keywords = item.Keywords,
            OpenGraphTitle = item.OpenGraphTitle, OpenGraphDescription = item.OpenGraphDescription, OpenGraphMediaId = item.OpenGraphMediaId };
        var input = new LocationInput { Name = item.Name, Kind = item.Kind, ParentId = item.ParentId, Summary = item.Summary, Body = item.Body,
            LocalDetails = item.LocalDetails, IsActive = item.IsActive, IsReviewed = item.IsReviewed, FactsVerified = item.FactsVerified, Seo = seo,
            PracticeAreaIds = item.PracticeLinks.Select(x => x.PracticeAreaId).ToArray(), ArticleIds = item.ArticleLinks.Select(x => x.ArticleId).ToArray() };
        return new(item.Id, item.Name, item.Slug, input, warnings.Count == 0, warnings, ancestors, areaLinks, articleLinks,
            item.OpenGraphMedia is { IsActive: true, IsDeleted: false } m ? "/uploads/" + m.StoragePath : null, item.UpdatedAt ?? item.CreatedAt);
    }
}
