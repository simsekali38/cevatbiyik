using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin/practice-areas")]
public sealed class PracticeAreasController(IPracticeAreaService areas, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        if (page < 1 || page > 100000)
        {
            return BadRequest();
        }

        ViewData["Page"] = page;
        return View(await areas.ListAsync(page, 20, cancellationToken));
    }
    [HttpGet("create")]
    public IActionResult Create() => View("Edit", new CreatePracticeAreaDto());
    [HttpGet("edit/{id:long}")]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken)
    {
        var item = await areas.GetAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        ViewData["EntityId"] = id;
        return View(new CreatePracticeAreaDto
        {
            Name = item.Name,
            ShortDescription = item.ShortDescription,
            Content = item.Content,
            SortOrder = item.SortOrder,
            IsActive = item.IsActive
        });
    }
    [HttpPost("save/{id:long?}")]
    public Task<IActionResult> Save([FromRoute] long? id, [Bind("Name,ShortDescription,Content,SortOrder,IsActive")] CreatePracticeAreaDto input,
        CancellationToken cancellationToken)
    {
        ViewData["EntityId"] = id;
        return SaveAsync(input, async () =>
        {
            if (id is null)
            {
                await areas.CreateAsync(input, cancellationToken);
            }
            else
            {
                await areas.UpdateAsync(id.Value, input, cancellationToken);
            }
        }, SecurityEvent.PracticeAreaSaved, cancellationToken);
    }
    [HttpPost("delete/{id:long}")]
    public Task<IActionResult> Delete(long id, bool confirmed, CancellationToken cancellationToken) =>
        confirmed ? ChangeAsync(() => areas.DeleteAsync(id, cancellationToken), SecurityEvent.PracticeAreaDeleted, cancellationToken)
            : Task.FromResult<IActionResult>(BadRequest());
}

