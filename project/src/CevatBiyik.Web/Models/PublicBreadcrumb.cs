using CevatBiyik.Application.DTOs;

namespace CevatBiyik.Web.Models;

public sealed record BreadcrumbItem(string Title, string? Url = null);

public static class PublicBreadcrumb
{
    public static IReadOnlyList<BreadcrumbItem> Create(string path, string title, object? model)
    {
        if (path == "/") { return []; }
        var sections = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["/makaleler"] = "Makaleler", ["/uzmanlik-alanlari"] = "Uzmanlık alanları",
            ["/konumlar"] = "Kayseri yerel bilgiler", ["/sss"] = "Sıkça Sorulan Sorular",
            ["/hakkimda"] = "Hakkımda", ["/iletisim"] = "İletişim",
            ["/kvkk"] = "KVKK / Aydınlatma", ["/cerez-politikasi"] = "Çerez Politikası",
            ["/yasal-uyari"] = "Yasal Uyarı"
        };
        var section = sections.Keys.FirstOrDefault(x => path == x || path.StartsWith(x + "/", StringComparison.Ordinal));
        if (section is null) { return []; }
        var items = new List<BreadcrumbItem> { new("Ana sayfa", "/") };
        if (path != section)
        {
            items.Add(new(sections[section], section));
            if (model is LocationDto location)
            {
                items.AddRange(location.Ancestors.Select(x => new BreadcrumbItem(x.Title, x.Path)));
            }
        }
        items.Add(new(string.IsNullOrWhiteSpace(title) ? sections[section] : title));
        return items;
    }
}
