using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Seo;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Controllers;
public sealed class FaqController(IFaqService faq) : PublicController
{
    [HttpGet("/sss")]
    public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
    {
        if (!ModelState.IsValid || page < 1 || page > 100000)
        {
            return BadRequest();
        }

        var result = await faq.ListAsync(page, true, ct);
        if (page > 1 && result.Items.Count == 0)
        {
            return NotFound();
        }

        SetPage("Sıkça Sorulan Sorular", "Yayımlanmış soru ve yanıtlar.", page == 1 ? "/sss" : $"/sss?page={page}", result.Items.Count == 0 || page > 1);
        if (result.Items.Count > 0)
        {
            ViewData["StructuredData"] = SeoPresentation.Faq(result.Items);
        }

        return View(result);
    }
}
