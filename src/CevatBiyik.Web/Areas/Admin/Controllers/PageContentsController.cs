using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Areas.Admin.Controllers;
[Route("admin/page-contents")]
public sealed class PageContentsController(IPageContentService pages, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct) => View(await pages.ListAsync(ct: ct));
    [HttpGet("edit/{id:long?}")]
    public async Task<IActionResult> Edit(long? id, string path = "/kvkk", CancellationToken ct = default)
    {
        ViewData["EntityId"] = id;
        if (id is null)
        {
            return ContentPublication.Pages.TryGetValue(path, out var title) ? View(new PageContentInput { Path = path, Title = title }) : BadRequest();
        }

        var item = await pages.GetAsync(id.Value, ct);
        return item is null ? NotFound() : View(new PageContentInput { Path = item.Path, Title = item.Title, Body = item.Body, IsActive = item.IsActive });
    }
    [HttpPost("save/{id:long?}")]
    public Task<IActionResult> Save(long? id, [Bind("Path,Title,Body,IsActive,IsReviewed")] PageContentInput input, CancellationToken ct)
    {
        ViewData["EntityId"] = id;
        return SaveAsync(input, () => pages.SaveAsync(id, input, ct), SecurityEvent.PageContentSaved, ct);
    }
    [HttpPost("disable/{id:long}")]
    public Task<IActionResult> Disable(long id, bool confirmed, CancellationToken ct) => confirmed
        ? ChangeAsync(() => pages.DisableAsync(id, ct), SecurityEvent.PageContentDisabled, ct) : Task.FromResult<IActionResult>(BadRequest());
}
