using System.Globalization;
using System.Xml.Linq;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Seo;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Controllers;
public sealed class SeoController(ISitemapService sitemap, IConfiguration configuration, IProfileService profiles, IPageContentService pages, IFaqService faq, ISettingService settings) : Controller
{
    private static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
    private bool Indexing => configuration.GetValue<bool>("Site:AllowIndexing");
    [HttpGet("/robots.txt")]
    public IActionResult Robots() => Content(Indexing
        ? "User-agent: *\nDisallow: /admin\nDisallow: /hata\nSitemap: " + SeoPresentation.Origin(configuration) + "/sitemap.xml\n"
        : "User-agent: *\nDisallow: /\n", "text/plain; charset=utf-8");
    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!Indexing)
        {
            return Xml(new XElement(Ns + "urlset"));
        }

        var counts = await sitemap.CountAsync(cancellationToken);
        var total = (counts.Articles + 999) / 1000 + (counts.PracticeAreas + 999) / 1000;
        if (total >= 50000)
        {
            return StatusCode(503);
        }

        var origin = SeoPresentation.Origin(configuration);
        var root = new XElement(Ns + "sitemapindex", new XElement(Ns + "sitemap", new XElement(Ns + "loc", origin + "/sitemaps/pages.xml")));
        foreach (var (kind, count) in new[] { ("articles", counts.Articles), ("practice-areas", counts.PracticeAreas) })
        {
            for (var page = 1L; page <= (count + 999) / 1000; page++)
            {
                root.Add(new XElement(Ns + "sitemap", new XElement(Ns + "loc", origin + $"/sitemaps/{kind}/{page}.xml")));
            }
        }

        return Xml(root);
    }
    [HttpGet("/sitemaps/pages.xml")]
    public async Task<IActionResult> Pages(CancellationToken ct)
    {
        if (!Indexing)
        {
            return NotFound();
        }

        var entries = new List<SitemapEntry> { new("/", null), new("/makaleler", null), new("/uzmanlik-alanlari", null) };
        var values = await settings.ListPublicAsync(ct);
        var site = new CevatBiyik.Web.Models.PublicSiteSettings(values.ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal));
        if (SeoPresentation.ContactFactsComplete(site))
        {
            entries.Add(new("/iletisim", null));
        }
        if (await profiles.GetAsync(true, ct) is not null)
        {
            entries.Add(new("/hakkimda", null));
        }

        foreach (var item in await pages.ListAsync(true, ct))
        {
            if (item.Path != "/")
            {
                entries.Add(new(item.Path, item.UpdatedAt));
            }
        }

        if ((await faq.ListAsync(publicOnly: true, ct: ct)).Items.Count > 0)
        {
            entries.Add(new("/sss", null));
        }

        return UrlSet(entries);
    }
    [HttpGet("/sitemaps/{kind}/{page:int}.xml")]
    public async Task<IActionResult> Entries(string kind, int page, CancellationToken cancellationToken)
    {
        if (!Indexing || kind is not ("articles" or "practice-areas") || page < 1 || page > 100000)
        {
            return NotFound();
        }

        var entries = await sitemap.ListAsync(kind == "articles", page, cancellationToken);
        return entries.Count == 0 ? NotFound() : UrlSet(entries);
    }
    private ContentResult UrlSet(IEnumerable<SitemapEntry> entries)
    {
        var origin = SeoPresentation.Origin(configuration);
        return Xml(new XElement(Ns + "urlset", entries.Select(x => new XElement(Ns + "url",
            new XElement(Ns + "loc", origin + x.Path), x.ModifiedAt is { } modified ? new XElement(Ns + "lastmod", modified.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) : null))));
    }
    private ContentResult Xml(XElement root) => Content(new XDocument(new XDeclaration("1.0", "utf-8", null), root).ToString(), "application/xml; charset=utf-8");
}
