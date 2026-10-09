using CevatBiyik.Application.DTOs;
using CevatBiyik.Web.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
namespace CevatBiyik.Web.Seo;
public static class SeoPresentation
{
    public static string Origin(IConfiguration configuration)
    {
        var value = configuration["Site:BaseUrl"];
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == "https" && string.IsNullOrEmpty(uri.UserInfo)
            ? uri.GetLeftPart(UriPartial.Authority) : "https://cevatbiyik.av.tr";
    }
    public static void Apply(ViewDataDictionary data, SeoInput seo, string? image)
    {
        if (!string.IsNullOrWhiteSpace(seo.MetaTitle))
        {
            data["SeoTitle"] = seo.MetaTitle;
        }

        if (!string.IsNullOrWhiteSpace(seo.MetaDescription))
        {
            data["MetaDescription"] = seo.MetaDescription;
        }

        if (!string.IsNullOrWhiteSpace(seo.Keywords))
        {
            data["Keywords"] = seo.Keywords;
        }

        if (!string.IsNullOrWhiteSpace(seo.OpenGraphTitle))
        {
            data["OpenGraphTitle"] = seo.OpenGraphTitle;
        }

        if (!string.IsNullOrWhiteSpace(seo.OpenGraphDescription))
        {
            data["OpenGraphDescription"] = seo.OpenGraphDescription;
        }

        if (image is not null && image.StartsWith("/uploads/images/", StringComparison.Ordinal))
        {
            data["OpenGraphImage"] = image;
        }
    }
    public static object Article(ArticleDto article, string origin) => new Dictionary<string, object?>
    {
        ["@context"] = "https://schema.org",
        ["@type"] = "Article",
        ["headline"] = article.Title,
        ["description"] = article.Excerpt,
        ["datePublished"] = article.PublishedAt?.ToString("O"),
        ["dateModified"] = (article.UpdatedAt ?? article.CreatedAt).ToString("O"),
        ["mainEntityOfPage"] = origin + "/makaleler/" + Uri.EscapeDataString(article.Slug),
        ["author"] = new Dictionary<string, string> { ["@type"] = "Person", ["name"] = "Av. Cevat Bıyık" },
        ["image"] = article.Cover is null ? null : origin + article.Cover.Url
    };
    public static object Breadcrumb(string origin, string section, string path, string title, string detail) => new Dictionary<string, object?>
    {
        ["@context"] = "https://schema.org",
        ["@type"] = "BreadcrumbList",
        ["itemListElement"] = new[]
        {
            new Dictionary<string, object> { ["@type"] = "ListItem", ["position"] = 1, ["name"] = "Ana sayfa", ["item"] = origin + "/" },
            new Dictionary<string, object> { ["@type"] = "ListItem", ["position"] = 2, ["name"] = section, ["item"] = origin + path },
            new Dictionary<string, object> { ["@type"] = "ListItem", ["position"] = 3, ["name"] = title, ["item"] = origin + detail }
        }
    };
    public static object? LegalService(PublicSiteSettings site, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("Site:LegalServiceSchemaEnabled") || new[] { site.Name, site.Email, site.Phone, site.Address }.Any(x =>
            string.IsNullOrWhiteSpace(x) || x.Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        return new Dictionary<string, object>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "LegalService",
            ["name"] = site.Name,
            ["url"] = Origin(configuration),
            ["email"] = site.Email,
            ["telephone"] = site.Phone,
            ["address"] = site.Address
        };
    }
}
