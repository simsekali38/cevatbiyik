using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Seo;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin/seo")]
public sealed class SeoController(ISeoAuditService audit, IConfiguration configuration, IWebHostEnvironment environment, IAdminOperation operations) : AdminControllerBase(operations)
{
    [HttpGet("")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || page is < 1 or > 100000)
        {
            return BadRequest();
        }

        ViewData["AllowIndexing"] = configuration.GetValue<bool>("Site:AllowIndexing");
        ViewData["OriginValid"] = SeoPolicy.TryOrigin(configuration["Site:BaseUrl"], out _);
        ViewData["CanonicalOrigin"] = SeoPresentation.Origin(configuration);
        ViewData["Production"] = environment.IsProduction();
        ViewData["LegalServiceEnabled"] = configuration.GetValue<bool>("Site:LegalServiceSchemaEnabled");
        return View(await audit.ReadAsync(page, ct));
    }
}
