using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;
[Route("admin/page-seo")]
public sealed class PageSeoController(IPageSeoService seo, IMediaService media, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) => View(await seo.ListAsync(cancellationToken));
    [HttpGet("edit/{id:long?}")]
    public async Task<IActionResult> Edit(long? id, int choicesPage = 1, CancellationToken cancellationToken = default)
    {
        if (choicesPage < 1 || choicesPage > 100000)
        {
            return BadRequest();
        }

        ViewData["EntityId"] = id;
        ViewData["ChoicesPage"] = choicesPage;
        ViewData["Media"] = await media.ListAsync(choicesPage, cancellationToken);
        if (id is null)
        {
            return View(new PageSeoInput());
        }

        var item = await seo.GetAsync(id.Value, cancellationToken);
        return item is null ? NotFound() : View(new PageSeoInput { Path = item.Path, Seo = item.Seo, IsActive = item.IsActive });
    }
    [HttpPost("save/{id:long?}")]
    public async Task<IActionResult> Save(long? id, [Bind("Path,Seo,IsActive")] PageSeoInput input, CancellationToken cancellationToken)
    {
        ViewData["EntityId"] = id; ViewData["Media"] = await media.ListAsync(cancellationToken: cancellationToken);
        return await SaveAsync(input, () => seo.SaveAsync(id, input, cancellationToken), SecurityEvent.PageSeoSaved, cancellationToken);
    }
    [HttpPost("disable/{id:long}")]
    public Task<IActionResult> Disable(long id, bool confirmed, CancellationToken cancellationToken) => confirmed
        ? ChangeAsync(() => seo.DisableAsync(id, cancellationToken), SecurityEvent.PageSeoDisabled, cancellationToken)
        : Task.FromResult<IActionResult>(BadRequest());
}
