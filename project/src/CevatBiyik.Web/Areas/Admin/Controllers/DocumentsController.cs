using System.Security.Claims;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;
[Route("admin/documents")]
public sealed class DocumentsController(IDocumentService documents, IFileCatalog catalog, IDocumentStorage storage, IAdminOperation operations, IAuditLogService audit) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(bool deleted = false, int page = 1, CancellationToken ct = default)
    {
        if (page < 1 || page > 100000)
        {
            return BadRequest();
        }

        ViewData["Deleted"] = deleted; ViewData["Page"] = page;
        return View(await documents.ListAsync(deleted, page, ct));
    }
    [HttpPost("upload"), RequestSizeLimit(6291456), RequestFormLimits(MultipartBodyLengthLimit = 6291456)]
    public async Task<IActionResult> Upload(IFormFile? file, string? title, CancellationToken ct)
    {
        DocumentDto? uploaded = null;
        string? path = null;
        var committed = false;
        try
        {
            if (file is null || file.Length is <= 0 or > 5242880)
            {
                throw new ValidationException("En fazla 5 MiB boyutunda PDF seçin.");
            }

            await using var content = file.OpenReadStream();
            await Operations.ExecuteAsync(SecurityEvent.DocumentUploaded, User.FindFirstValue(ClaimTypes.NameIdentifier)!, async () =>
            {
                uploaded = await documents.UploadAsync(content, file.FileName, file.ContentType, title ?? "", ct);
                path = (await catalog.DocumentAsync(uploaded.Id, ct))!.StoragePath;
            }, ct);
            committed = true; TempData["AdminSuccess"] = "Özel belge yüklendi.";
        }
        catch (ValidationException exception) { TempData["AdminError"] = exception.Message; }
        finally { if (!committed && path is not null)
            {
                await storage.DeleteAsync(path, CancellationToken.None);
            }
        }
        return RedirectToAction(nameof(Index));
    }
    [HttpGet("download/{id:long}")]
    public async Task<IActionResult> Download(long id, CancellationToken ct)
    {
        try
        {
            var stream = await documents.DownloadAsync(id, ct);
            try { await audit.WriteAsync(SecurityEvent.DocumentDownloaded, User.FindFirstValue(ClaimTypes.NameIdentifier), ct); }
            catch { await stream.DisposeAsync(); throw; }
            Response.Headers.CacheControl = "no-store, private";
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            Response.Headers["Content-Security-Policy"] = "sandbox; default-src 'none'";
            return File(stream, "application/pdf", $"document-{id}.pdf", false);
        }
        catch (NotFoundException) { return NotFound(); }
        catch (ValidationException) { return StatusCode(409, "Belge doğrulanamadı veya dosya eksik."); }
    }
    [HttpPost("delete/{id:long}")]
    public Task<IActionResult> Delete(long id, bool confirmed, CancellationToken ct) => confirmed
        ? ChangeAsync(() => documents.DeleteAsync(id, ct), SecurityEvent.DocumentDeleted, ct) : Task.FromResult<IActionResult>(BadRequest());
    [HttpPost("restore/{id:long}")]
    public async Task<IActionResult> Restore(long id, bool confirmed, CancellationToken ct)
    {
        if (!confirmed)
        {
            return BadRequest();
        }

        try { return await ChangeAsync(() => documents.RestoreAsync(id, ct), SecurityEvent.DocumentRestored, ct); }
        catch (ValidationException exception) { TempData["AdminError"] = exception.Message; return RedirectToAction(nameof(Index), new { deleted = true }); }
    }
}
