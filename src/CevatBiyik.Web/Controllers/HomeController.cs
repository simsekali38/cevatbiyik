using Microsoft.AspNetCore.Mvc;
using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Models;

namespace CevatBiyik.Web.Controllers;

public sealed class HomeController(IArticleService articles, IPracticeAreaService practiceAreas,
    IWebHostEnvironment environment, IConfiguration configuration) : PublicController
{
    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        SetPage("Ana sayfa", "Av. Cevat Bıyık — makaleler, çalışma alanları ve iletişim.", "/");
        // Sequential queries share the same scoped EF context.
        var recent = await articles.SearchPublishedAsync(pageSize: 3, cancellationToken: cancellationToken);
        var areas = await practiceAreas.ListActiveAsync(pageSize: 3, cancellationToken: cancellationToken);
        return View(new HomeViewModel(recent.Items, areas.Items));
    }

    [HttpGet("/hakkimda")]
    public IActionResult About()
    {
        SetPage("Hakkımda", "Av. Cevat Bıyık hakkında bilgiler.", "/hakkimda");
        return View();
    }

    [HttpGet("/iletisim")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Contact()
    {
        SetPage("İletişim", "Av. Cevat Bıyık iletişim bilgileri ve iletişim formu.", "/iletisim");
        return View(ContactFormPolicy.Create(new CreateContactMessageDto(), environment, configuration));
    }
}
