using System.Text.RegularExpressions;
using CevatBiyik.Application.Interfaces;

namespace CevatBiyik.Web.Analytics;

public sealed partial class AnalyticsPolicy(IConfiguration config, IPageContentService pages)
{
    public const string ItemKey = "AnalyticsAvailable";
    public string Version => config["Analytics:ConsentVersion"] ?? "";
    public string Notice => config["Analytics:Notice"] ?? "";
    public string? GtmId => config["Analytics:GtmId"];
    public string? Ga4Id => config["Analytics:Ga4Id"];
    public bool Configured => config.GetValue<bool>("Analytics:Enabled") && config.GetValue<bool>("Analytics:LegalReviewConfirmed") &&
        VersionPattern().IsMatch(Version) && Notice.Length is >= 20 and <= 1000 &&
        !Notice.Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase) &&
        !Notice.Any(c => c is '<' or '>' || char.IsControl(c)) &&
        (GtmPattern().IsMatch(GtmId ?? "") && string.IsNullOrEmpty(Ga4Id) ||
         Ga4Pattern().IsMatch(Ga4Id ?? "") && string.IsNullOrEmpty(GtmId));
    public async Task<bool> AvailableAsync(HttpContext context)
    {
        if (context.Items.TryGetValue(ItemKey, out var cached)) { return cached is true; }
        var available = Configured && !context.Request.Path.StartsWithSegments("/admin") &&
            !context.Request.Path.StartsWithSegments("/hata") &&
            await pages.PublishedAsync("/cerez-politikasi", context.RequestAborted) is not null &&
            await pages.PublishedAsync("/kvkk", context.RequestAborted) is not null;
        context.Items[ItemKey] = available;
        return available;
    }
    [GeneratedRegex("^[A-Za-z0-9_-]{1,32}\\z", RegexOptions.CultureInvariant)] private static partial Regex VersionPattern();
    [GeneratedRegex("^GTM-[A-Z0-9]{4,20}\\z", RegexOptions.CultureInvariant)] private static partial Regex GtmPattern();
    [GeneratedRegex("^G-[A-Z0-9]{4,20}\\z", RegexOptions.CultureInvariant)] private static partial Regex Ga4Pattern();
}
