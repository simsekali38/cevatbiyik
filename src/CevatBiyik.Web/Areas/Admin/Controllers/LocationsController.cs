using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Areas.Admin.Controllers;
[Route("admin/locations")]
public sealed class LocationsController(ILocationService locations, IPracticeAreaService areas, IArticleService articles,
    IMediaService media, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || page is < 1 or > 100000)
        {
            return BadRequest();
        }
        return View(await locations.ListAsync(page, ct: ct));
    }
    [HttpGet("edit/{id:long?}")]
    public async Task<IActionResult> Edit(long? id, int choicesPage = 1, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || choicesPage is < 1 or > 100000)
        {
            return BadRequest();
        }
        var item = id.HasValue ? await locations.GetAsync(id.Value, ct) : null;
        if (id.HasValue && item is null)
        {
            return NotFound();
        }
        ViewData["EntityId"] = id; ViewData["QualityWarnings"] = item?.Warnings; ViewData["ChoicesPage"] = choicesPage;
        await ChoicesAsync(choicesPage, ct);
        return View(item?.Input ?? new LocationInput());
    }
    [HttpPost("save/{id:long?}")]
    public async Task<IActionResult> Save(long? id, [Bind("Name,Kind,ParentId,Summary,Body,LocalDetails,IsActive,IsReviewed,FactsVerified,Seo,PracticeAreaIds,ArticleIds")] LocationInput input, CancellationToken ct)
    {
        ViewData["EntityId"] = id; await ChoicesAsync(1, ct);
        return await SaveAsync(input, () => locations.SaveAsync(id, input, ct), SecurityEvent.LocationSaved, ct);
    }
    [HttpPost("delete/{id:long}")]
    public Task<IActionResult> Delete(long id, bool confirmed, CancellationToken ct) => confirmed
        ? ChangeAsync(() => locations.DeleteAsync(id, ct), SecurityEvent.LocationDeleted, ct) : Task.FromResult<IActionResult>(BadRequest());
    private async Task ChoicesAsync(int page, CancellationToken ct)
    {
        // Bounded editor pages; selected IDs absent from a choice page remain explicitly visible in the form.
        ViewData["Locations"] = await locations.ChoicesAsync(ct);
        ViewData["Areas"] = (await areas.ListActiveAsync(page, 20, ct)).Items;
        ViewData["Articles"] = (await articles.SearchPublishedAsync(page: page, pageSize: 20, cancellationToken: ct)).Items;
        ViewData["Media"] = await media.ListAsync(page, ct);
    }
}
