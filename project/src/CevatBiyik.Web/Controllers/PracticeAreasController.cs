using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Controllers;

[Route("uzmanlik-alanlari")]
public sealed class PracticeAreasController(IPracticeAreaService practiceAreas, IConfiguration configuration, ILocationService locations) : PublicController
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid || !ValidPage(page))
        {
            return BadRequest();
        }

        SetPage("Uzmanlık alanları", "Av. Cevat Bıyık tarafından yayımlanan çalışma alanları.",
            page == 1 ? "/uzmanlik-alanlari" : $"/uzmanlik-alanlari?page={page}");
        return View(await practiceAreas.ListActiveAsync(page, cancellationToken: cancellationToken));
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        if (!ValidSlug(slug))
        {
            return NotFound();
        }

        var area = await practiceAreas.GetActiveBySlugAsync(slug, cancellationToken);
        if (area is null)
        {
            return NotFound();
        }

        if (!string.Equals(slug, area.Slug, StringComparison.Ordinal))
        {
            return RedirectToActionPermanent(nameof(Detail), new { slug = area.Slug });
        }

        SetPage(area.Name, Description(area.ShortDescription, area.Name), "/uzmanlik-alanlari/" + Uri.EscapeDataString(area.Slug));
        CevatBiyik.Web.Seo.SeoPresentation.Apply(ViewData, area.Seo, area.OpenGraphImageUrl);
        ViewData["StructuredData"] = CevatBiyik.Web.Seo.SeoPresentation.Breadcrumb(CevatBiyik.Web.Seo.SeoPresentation.Origin(configuration),
            "Uzmanlık alanları", "/uzmanlik-alanlari", area.Name, "/uzmanlik-alanlari/" + Uri.EscapeDataString(area.Slug));
        ViewData["RelatedLocations"] = await locations.RelatedAsync(practiceAreaId: area.Id, ct: cancellationToken);
        return View(area);
    }
}
