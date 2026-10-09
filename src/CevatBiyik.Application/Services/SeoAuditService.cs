using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
namespace CevatBiyik.Application.Services;

public sealed class SeoAuditService(IPageSeoService pages, IArticleService articles, IPracticeAreaService areas,
    ISettingService settings, IPageContentService contents) : ISeoAuditService
{
    public async Task<SeoAuditResult> ReadAsync(int page = 1, CancellationToken ct = default)
    {
        ServiceRules.ValidatePage(page, 12);
        var rows = new List<SeoAuditRow>();
        var fixedPages = await pages.ListAsync(ct);
        var siteName = (await settings.ListPublicAsync(ct)).SingleOrDefault(x => x.Key == "Site.Name")?.Value ?? "Av. Cevat Bıyık";
        var publicContents = await contents.ListAsync(true, ct);
        foreach (var path in PageSeoValidator.Paths)
        {
            var item = fixedPages.SingleOrDefault(x => x.Path == path);
            var fallback = publicContents.SingleOrDefault(x => x.Path == path && path != "/")?.Title ?? path switch
            { "/" => "Ana sayfa", "/hakkimda" => "Hakkımda", "/iletisim" => "İletişim", "/makaleler" => "Makaleler",
                "/uzmanlik-alanlari" => "Uzmanlık alanları", "/sss" => "Sıkça Sorulan Sorular", "/kvkk" => "KVKK / Aydınlatma",
                "/cerez-politikasi" => "Çerez Politikası", _ => "Yasal Uyarı" };
            rows.Add(new(path, path, item is null ? "/admin/page-seo/edit" : "/admin/page-seo/edit/" + item.Id,
                SeoQuality.Check(item is { IsActive: true } ? item.Seo : new SeoInput(), fallback + " | " + siteName)));
        }
        var articleRows = await articles.ListAsync(page, 12, ct);
        foreach (var item in articleRows)
        {
            rows.Add(new(item.Title, "/makaleler/" + item.Slug, "/admin/articles/edit/" + item.Id, SeoQuality.Check(item.Seo, item.Title + " | " + siteName)));
        }

        var areaRows = await areas.ListAsync(page, 12, ct);
        foreach (var item in areaRows)
        {
            rows.Add(new(item.Name, "/uzmanlik-alanlari/" + item.Slug, "/admin/practice-areas/edit/" + item.Id, SeoQuality.Check(item.Seo, item.Name + " | " + siteName)));
        }

        return new(rows, page, articleRows.Count == 12 || areaRows.Count == 12);
    }
}
