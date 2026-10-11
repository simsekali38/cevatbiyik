using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
using CevatBiyik.Web.Models;

namespace CevatBiyik.Web.Seo;

public sealed record DiscoveryPage(string Path, string Title, DateTime? ModifiedAt = null);
public sealed record DiscoveryPages(string Name, string Description, IReadOnlyList<DiscoveryPage> Pages);

/// <summary>Shared published-page eligibility for sitemap and public discovery documents.</summary>
public sealed class PublicDiscoveryService(IProfileService profiles, IPageContentService pages,
    IFaqService faq, ISettingService settings, ILocationService locations)
{
    public async Task<DiscoveryPages> ReadAsync(CancellationToken ct)
    {
        var values = await settings.ListPublicAsync(ct);
        var site = new PublicSiteSettings(values.ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal));
        var entries = new List<DiscoveryPage>
        {
            new("/", "Ana sayfa"), new("/makaleler", "Makaleler"), new("/uzmanlik-alanlari", "Uzmanlık alanları")
        };
        if (SeoPresentation.ContactFactsComplete(site)) { entries.Add(new("/iletisim", "İletişim")); }
        if (await profiles.GetAsync(true, ct) is not null) { entries.Add(new("/hakkimda", "Hakkımda")); }
        foreach (var item in await pages.ListAsync(true, ct))
        {
            if (item.Path != "/" && ContentPublication.Pages.ContainsKey(item.Path))
            {
                entries.Add(new(item.Path, RichText.ToPlainText(item.Title), item.UpdatedAt));
            }
        }
        if ((await faq.ListAsync(publicOnly: true, ct: ct)).Items.Count > 0) { entries.Add(new("/sss", "Sıkça sorulan sorular")); }
        if ((await locations.IndexableAsync(ct)).Count > 0) { entries.Add(new("/konumlar", "Yerel bilgiler")); }
        return new(site.Name, site.Description, entries.DistinctBy(x => x.Path, StringComparer.Ordinal).ToArray());
    }
}
