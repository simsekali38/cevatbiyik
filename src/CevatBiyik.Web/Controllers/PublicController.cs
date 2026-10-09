using CevatBiyik.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Controllers;

[ServiceFilter(typeof(PublicSiteFilter))]
public abstract class PublicController : Controller
{
    protected void SetPage(string title, string description, string canonicalPath, bool noIndex = false)
    {
        ViewData["Title"] = title;
        ViewData["MetaDescription"] = description;
        ViewData["CanonicalPath"] = canonicalPath;
        if (noIndex)
        {
            ViewData["Robots"] = "noindex, follow";
        }
    }

    protected static bool ValidSlug(string slug) => slug.Length is > 0 and <= 200 &&
        slug.All(c => char.IsAsciiLetterOrDigit(c) || c == '-') && slug[0] != '-' && slug[^1] != '-';
    protected static bool ValidPage(int page) => page is >= 1 and <= int.MaxValue / 12;
    protected static string Description(string? text, string fallback)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return fallback;
        }

        return text.Length > 160 ? text[..157] + "…" : text;
    }
}
