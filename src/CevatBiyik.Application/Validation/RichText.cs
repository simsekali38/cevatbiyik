using System.Net;
using System.Text.Encodings.Web;
using AngleSharp.Html.Parser;
using Ganss.Xss;

namespace CevatBiyik.Application.Validation;

/// <summary>One allowlist for CMS input, canonical persistence and defensive output. Contact/meta remain plain text.</summary>
public static class RichText
{
    private static readonly string[] Tags = ["p", "br", "h2", "h3", "strong", "b", "em", "i", "u", "ul", "ol", "li", "blockquote", "a"];
    public static bool HasMarkup(string? value) => value?.Contains('<') is true;

    public static bool IsSafe(string? value)
    {
        if (value is null) { return true; }
        if (value.Length > 100000) { return false; }
        if (!HasMarkup(value)) { return PlainText.IsSafe(value); }
        var clean = Clean(value, out var removed);
        var text = ToPlainText(clean);
        return !removed && !string.IsNullOrWhiteSpace(text) && PlainText.IsSafe(text);
    }

    public static bool Fits(string? value, int limit) => value is null || (value.Length <= limit && Normalize(value).Length <= limit);

    public static string Normalize(string value) => HasMarkup(value) ? Clean(value, out _).Trim() : value.Trim();

    public static string Render(string? value)
    {
        if (string.IsNullOrEmpty(value)) { return string.Empty; }
        // Never parse an unbounded legacy fragment, and never trust previously stored HTML.
        return HasMarkup(value) && value.Length <= 100000 ? Clean(value, out _) : HtmlEncoder.Default.Encode(value);
    }

    public static string ToPlainText(string? value)
    {
        if (string.IsNullOrEmpty(value)) { return string.Empty; }
        if (!HasMarkup(value) || value.Length > 100000) { return WebUtility.HtmlDecode(value); }
        var document = new HtmlParser().ParseDocument(Clean(value, out _));
        foreach (var element in document.QuerySelectorAll("p,h2,h3,li,blockquote,br"))
        {
            element.Parent?.InsertBefore(document.CreateTextNode("\n"), element.NextSibling);
        }
        return (document.Body?.TextContent ?? string.Empty).Trim();
    }

    public static bool IsAllowedLink(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsControl) || value.Contains('\\')) { return false; }
        if (value.StartsWith('/') && !value.StartsWith("//", StringComparison.Ordinal)) { return true; }
        if (value.StartsWith('#')) { return true; }
        return Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp) && string.IsNullOrEmpty(uri.UserInfo);
    }

    private static string Clean(string value, out bool removed)
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear(); sanitizer.AllowedTags.UnionWith(Tags);
        sanitizer.AllowedAttributes.Clear(); sanitizer.AllowedAttributes.Add("href"); sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedCssProperties.Clear(); sanitizer.AllowedAtRules.Clear();
        sanitizer.AllowedSchemes.Clear(); sanitizer.AllowedSchemes.UnionWith(["http", "https"]);
        sanitizer.AllowDataAttributes = false;
        var rejected = false;
        sanitizer.RemovingTag += (_, _) => rejected = true;
        sanitizer.RemovingAttribute += (_, _) => rejected = true;
        var clean = sanitizer.Sanitize(value);
        var document = new HtmlParser().ParseDocument(clean);
        foreach (var link in document.QuerySelectorAll("a[href]"))
        {
            if (!IsAllowedLink(link.GetAttribute("href"))) { link.RemoveAttribute("href"); rejected = true; }
        }
        foreach (var element in document.QuerySelectorAll("[class]"))
        {
            var permitted = element.LocalName is "p" or "h2" or "h3" or "li" or "blockquote";
            if (!permitted || element.GetAttribute("class") is not ("content-align-center" or "content-align-right" or "content-align-justify"))
            {
                element.RemoveAttribute("class"); rejected = true;
            }
        }
        removed = rejected;
        return document.Body?.InnerHtml ?? string.Empty;
    }
}
