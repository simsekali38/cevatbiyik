using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin/settings")]
public sealed class SettingsController(ISettingService settings, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        if (page < 1 || page > 100000)
        {
            return BadRequest();
        }

        ViewData["Page"] = page;
        return View((await settings.ListAsync(page, 20, cancellationToken)).Where(x => PublicSettingKeys.All.Contains(x.Key)).ToArray());
    }
    [HttpGet("create")]
    public IActionResult Create() => View("Edit", new UpdateSiteSettingDto());
    [HttpGet("edit/{id:long}")]
    public async Task<IActionResult> Edit(long id, CancellationToken cancellationToken)
    {
        var item = await settings.GetAsync(id, cancellationToken);
        if (item is null || !PublicSettingKeys.All.Contains(item.Key))
        {
            return NotFound();
        }

        ViewData["EntityId"] = id;
        return View(new UpdateSiteSettingDto { Key = item.Key, Value = item.Value, IsActive = item.IsActive });
    }
    [HttpPost("save/{id:long?}")]
    public async Task<IActionResult> Save([FromRoute] long? id, [Bind("Key,Value,IsActive")] UpdateSiteSettingDto input, CancellationToken cancellationToken)
    {
        if (id is { } entityId && !await IsManagedAsync(entityId, cancellationToken))
        {
            return NotFound();
        }
        ViewData["EntityId"] = id;
        return await SaveAsync(input, async () =>
        {
            if (id is null)
            {
                await settings.CreateAsync(input, cancellationToken);
            }
            else
            {
                await settings.UpdateAsync(id.Value, input, cancellationToken);
            }
        }, SecurityEvent.SettingSaved, cancellationToken);
    }
    [HttpPost("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, bool confirmed, CancellationToken cancellationToken)
    {
        if (!confirmed) { return BadRequest(); }
        if (!await IsManagedAsync(id, cancellationToken)) { return NotFound(); }
        return await ChangeAsync(() => settings.DeleteAsync(id, cancellationToken), SecurityEvent.SettingDeleted, cancellationToken);
    }

    private async Task<bool> IsManagedAsync(long id, CancellationToken cancellationToken)
    {
        var item = await settings.GetAsync(id, cancellationToken);
        return item is not null && PublicSettingKeys.All.Contains(item.Key);
    }
}
