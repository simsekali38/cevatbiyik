using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin/redirects")]
public sealed class RedirectsController(IRedirectService redirects, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default) => !ModelState.IsValid || page is < 1 or > 100000
        ? BadRequest() : View(await redirects.ListAsync(page, ct));
    [HttpGet("edit/{id:long?}")]
    public async Task<IActionResult> Edit(long? id, CancellationToken ct)
    {
        ViewData["EntityId"] = id;
        if (id is null)
        {
            return View(new RedirectInput());
        }

        var item = await redirects.GetAsync(id.Value, ct);
        return item is null ? NotFound() : View(new RedirectInput { SourcePath = item.SourcePath, DestinationPath = item.DestinationPath, IsActive = item.IsActive });
    }
    [HttpPost("save/{id:long?}")]
    public Task<IActionResult> Save(long? id, [Bind("SourcePath,DestinationPath,IsActive")] RedirectInput input, CancellationToken ct)
    {
        ViewData["EntityId"] = id;
        return SaveAsync(input, () => redirects.SaveAsync(id, input, ct), SecurityEvent.RedirectSaved, ct);
    }
    [HttpPost("disable/{id:long}")]
    public Task<IActionResult> Disable(long id, bool confirmed, CancellationToken ct) => confirmed
        ? ChangeAsync(() => redirects.DisableAsync(id, ct), SecurityEvent.RedirectDisabled, ct) : Task.FromResult<IActionResult>(BadRequest());
}
