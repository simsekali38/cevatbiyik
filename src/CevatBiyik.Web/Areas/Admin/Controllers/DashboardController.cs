using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin")]
public sealed class DashboardController(IAdminDashboardService dashboard, IAuditLogService audit, IAdminOperation operations)
    : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["RecentAudit"] = await audit.RecentAsync(cancellationToken);
        return View(await dashboard.GetAsync(cancellationToken));
    }
}
