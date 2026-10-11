using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin/contact-messages")]
public sealed class ContactMessagesController(IContactService messages, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        if (page < 1 || page > 100000)
        {
            return BadRequest();
        }

        ViewData["Page"] = page;
        return View(await messages.ListAsync(page, 20, cancellationToken));
    }
    [HttpPost("read/{id:long}")]
    public Task<IActionResult> Read(long id, bool isRead, CancellationToken cancellationToken) =>
        ChangeAsync(async () => { await messages.SetReadAsync(id, isRead, cancellationToken); }, SecurityEvent.ContactReadChanged, cancellationToken);
    [HttpPost("retry-notification/{id:long}")]
    public Task<IActionResult> RetryNotification(long id, bool confirmed, CancellationToken cancellationToken) =>
        confirmed ? ChangeAsync(() => messages.RetryNotificationAsync(id, cancellationToken), SecurityEvent.ContactNotificationRetried, cancellationToken, "Bildirim yeniden denemeye uygun değil. SMTP etkinliğini ve bildirim durumunu kontrol edin.")
            : Task.FromResult<IActionResult>(BadRequest());
    [HttpPost("delete/{id:long}")]
    public Task<IActionResult> Delete(long id, bool confirmed, CancellationToken cancellationToken) =>
        confirmed ? ChangeAsync(() => messages.DeleteAsync(id, cancellationToken), SecurityEvent.ContactDeleted, cancellationToken)
            : Task.FromResult<IActionResult>(BadRequest());
}

