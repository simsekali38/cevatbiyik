using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Route("admin/guide")]
public sealed class GuideController(IProfileService profiles, IPageContentService pages, ISettingService settings,
    IPracticeAreaService areas, IArticleService articles, IFaqService faq, IMediaService media, IAdminOperation operations)
    : AdminControllerBase(operations)
{
    private static readonly string[] ContactKeys = ["Contact.Email", "Contact.Phone", "Contact.Address"];
    private static readonly string[] LegalPaths = ["/kvkk", "/cerez-politikasi", "/yasal-uyari"];
    [HttpGet("")]
    public async Task<IActionResult> Index(int step = 1, CancellationToken ct = default)
    {
        if (step is < 1 or > 8) { return BadRequest(); }
        var profile = await profiles.GetAsync(true, ct);
        var publicPages = await pages.ListAsync(true, ct);
        var publicSettings = await settings.ListPublicAsync(ct);
        var activeAreas = await areas.ListActiveAsync(pageSize: 1, cancellationToken: ct);
        var published = await articles.SearchPublishedAsync(pageSize: 1, cancellationToken: ct);
        var questions = await faq.ListAsync(publicOnly: true, ct: ct);
        var images = await media.ListAsync(cancellationToken: ct);
        AdminGuideStep[] steps =
        [
            new(1, "İletişim bilgileri", "Ziyaretçinin size nasıl ulaşacağını belirleyin.", "/admin/settings", ContactKeys.All(key => publicSettings.Any(x => x.Key == key))),
            new(2, "Avukat profili", "Biyografi, vizyon ve doğrulanmış baro bilgisini girin.", "/admin/profile", profile is not null),
            new(3, "Hukuki metinler", "KVKK, çerez ve yasal uyarı metinlerini inceleyip yayınlayın.", "/admin/page-contents", LegalPaths.All(path => publicPages.Any(x => x.Path == path))),
            new(4, "Görsel kütüphanesi", "İçerik kapaklarını hazırlayın; açıklayıcı alternatif metin ekleyin.", "/admin/media", images.Count > 0),
            new(5, "Uzmanlık alanları", "Çalışma alanlarınızı ve açıklamalarını oluşturun.", "/admin/practice-areas", activeAreas.Items.Count > 0),
            new(6, "İlk makale", "Bir taslak oluşturun, önizleyin ve hazır olduğunda yayınlayın.", "/admin/articles", published.TotalCount > 0),
            new(7, "Sıkça sorulan sorular", "Ziyaretçilere yardımcı olacak soru ve yanıtları yayınlayın.", "/admin/faq", questions.TotalCount > 0),
            new(8, "Ana sayfa ve son kontrol", "Ana sayfa metnini, bağlantıları ve iletişim akışını kontrol edin.", "/admin/page-contents", publicPages.Any(x => x.Path == "/"))
        ];
        return View(new AdminGuideViewModel(steps, step));
    }
}
