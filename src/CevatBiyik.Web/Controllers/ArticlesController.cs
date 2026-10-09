using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Controllers;

[Route("makaleler")]
public sealed class ArticlesController(IArticleService articles, IPracticeAreaService practiceAreas, ITaxonomyService taxonomy, IConfiguration configuration) : PublicController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? q, long? category, long? topic, string? tag, int page = 1, int categoryPage = 1, int metadataPage = 1, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid || !ValidPage(page) || !ValidPage(categoryPage) || !ValidPage(metadataPage) || q?.Length > 100 || category is <= 0 || topic is <= 0 || tag?.Length > 200)
        {
            return BadRequest();
        }

        var result = await articles.SearchCmsPublishedAsync(q, category, topic, string.IsNullOrWhiteSpace(tag) ? null : tag, page, cancellationToken);
        var categories = await practiceAreas.ListActiveAsync(categoryPage, 12, cancellationToken);
        SetPage("Makaleler", "Yayımlanmış makaleleri inceleyin ve çalışma alanına göre filtreleyin.",
            page == 1 ? "/makaleler" : $"/makaleler?page={page}", !string.IsNullOrWhiteSpace(q) || category.HasValue || topic.HasValue || tag is not null || categoryPage > 1 || metadataPage > 1);
        return View(new ArticleListViewModel(result, q?.Trim(), category, categories, categoryPage)
        {
            Topics = await taxonomy.ListAsync(false, metadataPage, true, cancellationToken),
            Topic = topic,
            Tag = tag,
            Labels = await taxonomy.ListAsync(true, metadataPage, true, cancellationToken),
            MetadataPage = metadataPage
        });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        if (!ValidSlug(slug))
        {
            return NotFound();
        }

        var article = await articles.GetPublishedBySlugAsync(slug, cancellationToken);
        if (article is null)
        {
            return NotFound();
        }

        if (!string.Equals(slug, article.Slug, StringComparison.Ordinal))
        {
            return RedirectToActionPermanent(nameof(Detail), new { slug = article.Slug });
        }

        SetPage(article.Title, Description(article.Excerpt, article.Title), "/makaleler/" + Uri.EscapeDataString(article.Slug));
        ViewData["OpenGraphType"] = "article";
        CevatBiyik.Web.Seo.SeoPresentation.Apply(ViewData, article.Seo, article.OpenGraphImageUrl ?? article.Cover?.Url);
        var origin = CevatBiyik.Web.Seo.SeoPresentation.Origin(configuration);
        ViewData["StructuredData"] = new[] { CevatBiyik.Web.Seo.SeoPresentation.Article(article, origin),
            CevatBiyik.Web.Seo.SeoPresentation.Breadcrumb(origin, "Makaleler", "/makaleler", article.Title, "/makaleler/" + Uri.EscapeDataString(article.Slug)) };
        return View(article);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public IActionResult Search(string? q, long? category, long? topic, string? tag, int categoryPage = 1, int metadataPage = 1)
    {
        if (!ModelState.IsValid || q?.Length > 100 || category is <= 0 || topic is <= 0 || tag?.Length > 200 || !ValidPage(categoryPage) || !ValidPage(metadataPage))
        {
            return BadRequest();
        }

        // PRG keeps antiforgery tokens out of query strings and request URL logs.
        return RedirectToAction(nameof(Index), new { q = q?.Trim(), category, topic, tag = string.IsNullOrWhiteSpace(tag) ? null : tag, categoryPage, metadataPage });
    }
}
