using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Controllers;

[Route("makaleler")]
public sealed class ArticlesController(IArticleService articles, IPracticeAreaService practiceAreas) : PublicController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string? q, long? category, int page = 1, int categoryPage = 1, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid || !ValidPage(page) || !ValidPage(categoryPage) || q?.Length > 100 || category is <= 0)
        {
            return BadRequest();
        }

        var result = await articles.SearchPublishedAsync(q, category, page, cancellationToken: cancellationToken);
        var categories = await practiceAreas.ListActiveAsync(categoryPage, 12, cancellationToken);
        SetPage("Makaleler", "Yayımlanmış makaleleri inceleyin ve çalışma alanına göre filtreleyin.",
            page == 1 ? "/makaleler" : $"/makaleler?page={page}", !string.IsNullOrWhiteSpace(q) || category.HasValue || categoryPage > 1);
        return View(new ArticleListViewModel(result, q?.Trim(), category, categories, categoryPage));
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
        return View(article);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public IActionResult Search(string? q, long? category, int categoryPage = 1)
    {
        if (!ModelState.IsValid || q?.Length > 100 || category is <= 0 || !ValidPage(categoryPage))
        {
            return BadRequest();
        }

        // PRG keeps antiforgery tokens out of query strings and request URL logs.
        return RedirectToAction(nameof(Index), new { q = q?.Trim(), category, categoryPage });
    }
}
