using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin/taxonomy/{kind}")]
public sealed class TaxonomyController(ITaxonomyService taxonomy, IAdminOperation operations) : AdminControllerBase(operations)
{
    private static bool Valid(string kind) => kind is "categories" or "tags";
    [HttpGet("")]
    public async Task<IActionResult> Index(string kind, int page = 1, CancellationToken cancellationToken = default)
    {
        if (!Valid(kind) || page < 1 || page > 100000)
        {
            return BadRequest();
        }

        ViewData["Kind"] = kind; ViewData["Page"] = page;
        return View(await taxonomy.ListAsync(kind == "tags", page, cancellationToken: cancellationToken));
    }
    [HttpGet("edit/{id:long?}")]
    public async Task<IActionResult> Edit(string kind, long? id, CancellationToken cancellationToken)
    {
        if (!Valid(kind))
        {
            return BadRequest();
        }

        ViewData["Kind"] = kind; ViewData["EntityId"] = id;
        if (id is null)
        {
            return View(new TaxonomyInput());
        }

        var item = await taxonomy.GetAsync(kind == "tags", id.Value, cancellationToken);
        return item is null ? NotFound() : View(new TaxonomyInput { Name = item.Name, IsActive = item.IsActive });
    }
    [HttpPost("save/{id:long?}")]
    public Task<IActionResult> Save(string kind, long? id, [Bind("Name,IsActive")] TaxonomyInput input, CancellationToken cancellationToken)
    {
        if (!Valid(kind))
        {
            return Task.FromResult<IActionResult>(BadRequest());
        }

        ViewData["Kind"] = kind; ViewData["EntityId"] = id;
        return SaveAsync(input, async () => { await taxonomy.SaveAsync(kind == "tags", id, input, cancellationToken); }, SecurityEvent.TaxonomySaved, cancellationToken);
    }
    [HttpPost("delete/{id:long}")]
    public Task<IActionResult> Delete(string kind, long id, bool confirmed, CancellationToken cancellationToken) =>
        Valid(kind) && confirmed ? ChangeAsync(() => taxonomy.DeleteAsync(kind == "tags", id, cancellationToken), SecurityEvent.TaxonomyDeleted, cancellationToken)
        : Task.FromResult<IActionResult>(BadRequest());
}
