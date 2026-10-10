using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Models;
using CevatBiyik.Web.Seo;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Controllers;
[Route("konumlar")]
public sealed class LocationsController(ILocationService locations, IConfiguration configuration) : PublicController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || !ValidPage(page))
        {
            return BadRequest();
        }
        var result = await locations.ListAsync(page, true, ct);
        if (page > 1 && result.Items.Count == 0)
        {
            return NotFound();
        }
        SetPage("Kayseri yerel bilgiler", "İncelenmiş yerel bilgiler ve ilgili içerikler.", page == 1 ? "/konumlar" : $"/konumlar?page={page}", result.TotalCount == 0);
        return View(result);
    }
    [HttpGet("{slug}")]
    public async Task<IActionResult> Detail(string slug, CancellationToken ct)
    {
        if (!ValidSlug(slug) || await locations.PublishedAsync(slug.ToLowerInvariant(), ct) is not { } item)
        {
            return NotFound();
        }
        if (slug != item.Slug)
        {
            return RedirectToActionPermanent(nameof(Detail), new { slug = item.Slug });
        }
        SetPage(item.Name, item.Input.Summary, item.Path, !item.Indexable);
        SeoPresentation.Apply(ViewData, item.Input.Seo, item.OpenGraphImageUrl);
        var schemas = new List<object> { SeoPresentation.LocationBreadcrumb(SeoPresentation.Origin(configuration), item) };
        if (item.Indexable && ViewData["SiteSettings"] is PublicSiteSettings site && SeoPresentation.LocalLegalService(site, configuration, item) is { } legal)
        {
            schemas.Add(legal);
        }
        ViewData["StructuredData"] = schemas;
        ViewData["Children"] = (await locations.IndexableAsync(ct)).Where(x => x.Input.ParentId == item.Id).Select(x => new CevatBiyik.Application.DTOs.LocalLink(x.Id, x.Name, x.Path)).ToArray();
        return View(item);
    }
}
