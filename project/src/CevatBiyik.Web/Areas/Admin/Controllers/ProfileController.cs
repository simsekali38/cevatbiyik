using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Areas.Admin.Controllers;
[Route("admin/profile")]
public sealed class ProfileController(IProfileService profiles, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var item = await profiles.GetAsync(ct: ct);
        return View("Edit", item is null ? new ProfileInput() : new ProfileInput { Biography = item.Biography, Vision = item.Vision, BarAssociation = item.BarAssociation, IsActive = item.IsActive });
    }
    [HttpPost("save")]
    public Task<IActionResult> Save([Bind("Biography,Vision,BarAssociation,IsActive,IsReviewed")] ProfileInput input, CancellationToken ct) =>
        SaveAsync(input, () => profiles.SaveAsync(input, ct), SecurityEvent.ProfileSaved, ct);
}
