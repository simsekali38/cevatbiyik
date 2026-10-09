using System.Security.Claims;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;
[Route("admin/files")]
public sealed class FilesController(IFileMaintenance files, IAdminOperation operations, IAuditLogService audit) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(bool documents = false, long afterId = 0, string? afterPath = null, CancellationToken ct = default)
    {
        ViewData["Documents"] = documents;
        try { return View(await files.ReportAsync(documents, afterId, afterPath, ct)); }
        catch (ValidationException) { return BadRequest(); }
    }
    [HttpPost("quarantine")]
    public async Task<IActionResult> Quarantine(bool documents, string path, bool confirmed, CancellationToken ct)
    {
        if (!confirmed || !ModelState.IsValid || string.IsNullOrWhiteSpace(path) || path.Length > 100)
        {
            return BadRequest();
        }

        try
        {
            // Audit intent first: failed DB audit must never move a file.
            await audit.WriteAsync(SecurityEvent.OrphanQuarantineRequested, User.FindFirstValue(ClaimTypes.NameIdentifier), ct);
            await files.QuarantineAsync(documents, path, ct);
            TempData["AdminSuccess"] = "Sahipsiz dosya özel karantinaya taşındı.";
        }
        catch (ValidationException exception) { TempData["AdminError"] = exception.Message; }
        return RedirectToAction(nameof(Index), new { documents });
    }
    [HttpPost("restore-image")]
    public async Task<IActionResult> RestoreImage(long id, bool confirmed, CancellationToken ct)
    {
        if (!confirmed)
        {
            return BadRequest();
        }

        try { return await ChangeAsync(() => files.RestoreImageAsync(id, ct), SecurityEvent.MediaRestored, ct); }
        catch (ValidationException exception) { TempData["AdminError"] = exception.Message; return RedirectToAction(nameof(Index)); }
        catch (NotFoundException) { return NotFound(); }
    }
}
