using CevatBiyik.Application.DTOs;
using CevatBiyik.Web.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
namespace CevatBiyik.Web.Seo;
public static class SeoPresentation
{
    public static object Faq(IReadOnlyList<FaqDto> items) => new Dictionary<string, object>
    {
        ["@context"] = "https://schema.org", ["@type"] = "FAQPage",
        ["mainEntity"] = items.Select(x => new Dictionary<string, object>
        {
            ["@type"] = "Question", ["name"] = x.Question,
            ["acceptedAnswer"] = new Dictionary<string, string> { ["@type"] = "Answer", ["text"] = CevatBiyik.Application.Validation.RichText.ToPlainText(x.Answer) }
        }).ToArray()
    };
    public static string Origin(IConfiguration configuration)
    {
        var value = configuration["Site:BaseUrl"];
        return SeoPolicy.TryOrigin(value, out var uri) ? uri!.GetLeftPart(UriPartial.Authority) : "https://cevatbiyik.av.tr";
    }
    public static void Apply(ViewDataDictionary data, SeoInput seo, string? image)
    {
        if (!new CevatBiyik.Application.Validation.SeoValidator().Validate(seo).IsValid)
        {
            return;
        }

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

        if (SafeImage(image))
        {
            data["OpenGraphImage"] = image;
        }
    }
    private static bool SafeImage(string? image) => image is not null && System.Text.RegularExpressions.Regex.IsMatch(image,
        "^/uploads/images/[a-f0-9]{32}\\.webp$", System.Text.RegularExpressions.RegexOptions.CultureInvariant);
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
        ["image"] = SafeImage(article.Cover?.Url) ? origin + article.Cover!.Url : null
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
        if (!configuration.GetValue<bool>("Site:LegalServiceSchemaEnabled") || !ContactFactsComplete(site))
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
    public static object LocationBreadcrumb(string origin, LocationDto location)
    {
        var links = new[] { new LocalLink(0, "Ana sayfa", "/"), new LocalLink(0, "Kayseri yerel bilgiler", "/konumlar") }
            .Concat(location.Ancestors).Append(new(location.Id, location.Name, location.Path));
        return new Dictionary<string, object> { ["@context"] = "https://schema.org", ["@type"] = "BreadcrumbList",
            ["itemListElement"] = links.Select((x, i) => new Dictionary<string, object> { ["@type"] = "ListItem", ["position"] = i + 1, ["name"] = x.Title, ["item"] = origin + x.Path }).ToArray() };
    }
    public static object? LocalLegalService(PublicSiteSettings site, IConfiguration configuration, LocationDto location)
    {
        if (!location.Indexable || !location.Input.FactsVerified || LegalService(site, configuration) is not Dictionary<string, object> legal)
        {
            return null;
        }
        legal["@id"] = Origin(configuration) + "/#legal-service";
        legal["areaServed"] = new Dictionary<string, object> { ["@type"] = "AdministrativeArea", ["name"] = location.Name };
        return legal;
    }
    public static object? Person(PublicSiteSettings site, IConfiguration configuration, ProfileDto? profile)
    {
        if (!configuration.GetValue<bool>("Site:PersonSchemaEnabled") || profile is null || !profile.IsActive || !profile.IsReviewed || !ContactFactsComplete(site))
        {
            return null;
        }
        return new Dictionary<string, object> { ["@context"] = "https://schema.org", ["@type"] = "Person",
            ["@id"] = Origin(configuration) + "/#person", ["name"] = "Av. Cevat Bıyık", ["url"] = Origin(configuration) + "/hakkimda",
            ["description"] = profile.Biography, ["email"] = site.Email, ["telephone"] = site.Phone };
    }
    public static bool ContactFactsComplete(PublicSiteSettings site)
    {
        if (!CevatBiyik.Application.Validation.ContentPublication.Complete(site.Name, site.Email, site.Phone, site.Address) ||
            !System.Net.Mail.MailAddress.TryCreate(site.Email, out var email) || email.Address != site.Email ||
            site.Phone.Any(c => !char.IsAsciiDigit(c) && c is not '+' and not ' ' and not '(' and not ')' and not '-' and not '.'))
        {
            return false;
        }
        var phone = new string(site.Phone.Where(c => char.IsAsciiDigit(c) || c == '+').ToArray());
        var digits = phone.StartsWith('+') ? phone[1..] : phone;
        return digits.Length is >= 7 and <= 15 && digits.All(char.IsAsciiDigit);
    }
}
