using System.Globalization;
using System.Text;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Web.Seo;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Controllers;

/// <summary>Read-only metadata files, not a separate application API.</summary>
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class DiscoveryController(PublicDiscoveryService discovery, ISitemapService sitemap,
    IArticleService articles, IPracticeAreaService areas, ILocationService locations, IConfiguration configuration) : Controller
{
    private bool Enabled => configuration.GetValue<bool>("Site:AllowIndexing");
    private string Origin => SeoPresentation.Origin(configuration);

    [HttpGet("/ai-catalog.json")]
    public async Task<IActionResult> Catalog(int page = 1, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || page is < 1 or > 100000) { return BadRequest(); }
        if (!Enabled)
        {
            return Json(new { schema_version = "1.0", enabled = false, language = "tr", resources = Array.Empty<object>() });
        }

        var document = await discovery.ReadAsync(ct);
        var counts = await sitemap.CountAsync(ct);
        var articlePages = (counts.Articles + 999) / 1000;
        var areaPages = (counts.PracticeAreas + 999) / 1000;
        var totalPages = Math.Max(1, Math.Max(articlePages, areaPages));
        if (page > totalPages) { return NotFound(); }
        var resources = new List<object>();
        if (page == 1)
        {
            resources.AddRange(document.Pages.Select(x => (object)new { kind = "page", title = x.Title, url = Origin + x.Path, updated = x.ModifiedAt }));
            resources.AddRange((await locations.IndexableAsync(ct)).Select(x => (object)new { kind = "location", title = x.Name, url = Origin + x.Path, updated = x.ModifiedAt }));
        }
        foreach (var (kind, isArticle, count) in new[] { ("article", true, articlePages), ("practice_area", false, areaPages) })
        {
            if (page <= count)
            {
                resources.AddRange((await sitemap.ListAsync(isArticle, page, ct)).Select(x => (object)new { kind, url = Origin + x.Path, updated = x.ModifiedAt }));
            }
        }
        return Json(new
        {
            schema_version = "1.0", enabled = true, language = "tr", name = document.Name,
            description = document.Description, base_url = Origin, sitemap = Origin + "/sitemap.xml",
            llms = Origin + "/llms.txt", page, total_pages = totalPages,
            next = page < totalPages ? Origin + "/ai-catalog.json?page=" + (page + 1).ToString(CultureInfo.InvariantCulture) : null,
            content_scope = "published_public_content", legal_notice = "Genel bilgilendirme; kişiye özel hukuki görüş veya sonuç garantisi değildir.", resources
        });
    }

    [HttpGet("/llms.txt")]
    public async Task<IActionResult> Llms(CancellationToken ct)
    {
        if (!Enabled) { return Content("# Av. Cevat Bıyık\n\n> Bu ortamda kamuya açık içerik keşfi kapalıdır.\n", "text/plain; charset=utf-8"); }
        var document = await discovery.ReadAsync(ct);
        var output = new StringBuilder("# ").AppendLine(MarkdownText(document.Name)).AppendLine()
            .Append("> ").AppendLine(MarkdownText(document.Description)).AppendLine()
            .AppendLine("İçerikler genel bilgilendirmedir; kişiye özel hukuki görüş, resmî başvuru veya sonuç garantisi yerine geçmez.")
            .AppendLine().AppendLine("## Sayfalar");
        void Link(string title, string path) => output.Append("- [").Append(MarkdownText(title)).Append("](").Append(Origin).Append(path).AppendLine(")");
        foreach (var item in document.Pages) { Link(item.Title, item.Path); }
        output.AppendLine().AppendLine("## Çalışma alanları");
        foreach (var area in (await areas.ListActiveAsync(pageSize: 10, cancellationToken: ct)).Items)
        {
            if (ContentPublication.Complete(area.Name, area.Content)) { Link(area.Name, "/uzmanlik-alanlari/" + Uri.EscapeDataString(area.Slug)); }
        }
        output.AppendLine().AppendLine("## Son yazılar");
        foreach (var article in (await articles.SearchPublishedAsync(pageSize: 5, cancellationToken: ct)).Items)
        {
            if (ContentPublication.Complete(article.Title, article.Content)) { Link(article.Title, "/makaleler/" + Uri.EscapeDataString(article.Slug)); }
        }
        output.AppendLine().AppendLine("## Optional");
        Link("Tam içerik kataloğu", "/ai-catalog.json");
        Link("XML sitemap", "/sitemap.xml");
        Link("Tarama kuralları", "/robots.txt");
        return Content(output.ToString(), "text/plain; charset=utf-8");
    }

    private static string MarkdownText(string text) => RichText.ToPlainText(text)
        .Replace("\\", "\\\\", StringComparison.Ordinal).Replace("[", "\\[", StringComparison.Ordinal)
        .Replace("]", "\\]", StringComparison.Ordinal).Replace("\r", " ", StringComparison.Ordinal)
        .Replace("\n", " ", StringComparison.Ordinal).Trim();
}
