using System.Security.Claims;
using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;
[Route("admin/media")]
public sealed class MediaController(IMediaService media, IImageStorage storage, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        if (page < 1 || page > 100000)
        {
            return BadRequest();
        }

        ViewData["Page"] = page;
        return View(await media.ListAsync(page, cancellationToken));
    }
    [HttpPost("upload"), RequestSizeLimit(6291456), RequestFormLimits(MultipartBodyLengthLimit = 6291456)]
    public async Task<IActionResult> Upload(IFormFile? file, string? altText, CancellationToken cancellationToken)
    {
        MediaDto? uploaded = null;
        var committed = false;
        try
        {
            if (file is null || file.Length is <= 0 or > 5242880)
            {
                throw new ValidationException("En fazla 5 MiB boyutunda bir görsel seçin.");
            }

            await using var content = file.OpenReadStream();
            await Operations.ExecuteAsync(SecurityEvent.MediaUploaded, User.FindFirstValue(ClaimTypes.NameIdentifier)!, async () =>
            { uploaded = await media.UploadAsync(content, file.FileName, file.ContentType, altText ?? "", cancellationToken); }, cancellationToken);
            committed = true;
            TempData["AdminSuccess"] = "Görsel yüklendi.";
        }
        catch (ValidationException exception) { TempData["AdminError"] = exception.Message; }
        finally
        {
            // A failed audit rolls back the row; compensate the independently written filesystem asset.
            if (uploaded is not null && !committed)
            {
                await storage.DeleteAsync(uploaded.StoragePath, CancellationToken.None);
            }

        }
        return RedirectToAction(nameof(Index));
    }
    [HttpPost("delete/{id:long}")]
    public Task<IActionResult> Delete(long id, bool confirmed, CancellationToken cancellationToken) => confirmed
        ? ChangeAsync(() => media.DeleteAsync(id, cancellationToken), SecurityEvent.MediaDeleted, cancellationToken)
        : Task.FromResult<IActionResult>(BadRequest());
}
