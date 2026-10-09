using CevatBiyik.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CevatBiyik.Web.Controllers;
public sealed class PagesController(IPageContentService pages) : PublicController
{
    [HttpGet("/kvkk")]
    public Task<IActionResult> Privacy(CancellationToken ct) => Render("/kvkk", "KVKK / Aydınlatma", ct);
    [HttpGet("/cerez-politikasi")]
    public Task<IActionResult> Cookies(CancellationToken ct) => Render("/cerez-politikasi", "Çerez Politikası", ct);
    [HttpGet("/yasal-uyari")]
    public Task<IActionResult> Disclaimer(CancellationToken ct) => Render("/yasal-uyari", "Yasal Uyarı", ct);
    private async Task<IActionResult> Render(string path, string title, CancellationToken ct)
    {
        var item = await pages.PublishedAsync(path, ct);
        SetPage(item?.Title ?? title, title, path, item is null);
        ViewData["FallbackTitle"] = title;
        return View("Detail", item);
    }
}
