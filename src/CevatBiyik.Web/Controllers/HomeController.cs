using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Controllers;

public sealed class HomeController(IArticleService articles, IPracticeAreaService practiceAreas,
    IWebHostEnvironment environment, IConfiguration configuration, IProfileService profiles, IPageContentService pages) : PublicController
{
    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        SetPage("Ana sayfa", "Av. Cevat Bıyık — makaleler, çalışma alanları ve iletişim.", "/");
        // Sequential queries share the same scoped EF context.
        var recent = await articles.SearchPublishedAsync(pageSize: 3, cancellationToken: cancellationToken);
        var areas = await practiceAreas.ListActiveAsync(pageSize: 3, cancellationToken: cancellationToken);
        return View(new HomeViewModel(recent.Items, areas.Items, await pages.PublishedAsync("/", cancellationToken)));
    }

    [HttpGet("/hakkimda")]
    public async Task<IActionResult> About(CancellationToken cancellationToken)
    {
        var profile = await profiles.GetAsync(true, cancellationToken);
        SetPage("Hakkımda", "Av. Cevat Bıyık hakkında bilgiler.", "/hakkimda", profile is null);
        if (ViewData["SiteSettings"] is PublicSiteSettings site)
        {
            ViewData["StructuredData"] = CevatBiyik.Web.Seo.SeoPresentation.Person(site, configuration, profile);
        }
        return View(profile);
    }

    [HttpGet("/iletisim")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Contact(CancellationToken cancellationToken)
    {
        SetPage("İletişim", "Av. Cevat Bıyık iletişim bilgileri ve iletişim formu.", "/iletisim");
        var model = ContactFormPolicy.Create(new CreateContactMessageDto(), environment, configuration)
            with { Privacy = await pages.PublishedAsync("/kvkk", cancellationToken) };
        if (environment.IsProduction() && model.Privacy is null)
        {
            model = model with { CanSubmit = false };
        }

        return View(model);
    }
}
