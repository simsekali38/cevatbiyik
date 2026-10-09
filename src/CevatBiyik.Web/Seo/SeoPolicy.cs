namespace CevatBiyik.Web.Seo;

public static class SeoPolicy
{
    public static string Robots(bool allowIndexing, bool pageNoIndex) => !allowIndexing ? "noindex, nofollow"
        : pageNoIndex ? "noindex, follow" : "index, follow";
    public static bool TryOrigin(string? value, out Uri? uri)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out uri) && uri.Scheme == "https" && string.IsNullOrEmpty(uri.UserInfo)
            && uri.AbsolutePath == "/" && string.IsNullOrEmpty(uri.Query) && string.IsNullOrEmpty(uri.Fragment);
    }
    public static string? Canonical(string origin, string? path)
    {
        if (path is null || !path.StartsWith('/') || path.StartsWith("//", StringComparison.Ordinal) || path.Contains('\\') ||
            path.Contains('#') || path.Any(char.IsControl))
        {
            return null;
        }

        return Uri.TryCreate(origin + path, UriKind.Absolute, out var url) && url.GetLeftPart(UriPartial.Authority) == origin ? url.AbsoluteUri : null;
    }
}
