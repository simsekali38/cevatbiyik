using Microsoft.AspNetCore.DataProtection;

namespace CevatBiyik.Web.Analytics;

public sealed class AnalyticsConsent(IDataProtectionProvider protection, TimeProvider clock, AnalyticsPolicy policy)
{
    public const string CookieName = "__Host-CevatBiyik.Consent";
    private readonly IDataProtector protector = protection.CreateProtector("CevatBiyik.AnalyticsConsent.v1");
    public bool? Read(HttpContext context)
    {
        var value = context.Request.Cookies[CookieName];
        if (value is null || value.Length > 2048) { return null; }
        try
        {
            var parts = protector.Unprotect(value).Split('|');
            if (parts.Length != 3 || parts[0] != policy.Version || !long.TryParse(parts[2], out var ticks)) { return null; }
            var age = clock.GetUtcNow().UtcDateTime.Ticks - ticks;
            if (age < 0 || age > TimeSpan.FromDays(180).Ticks) { return null; }
            return parts[1] switch { "allow" => true, "deny" => false, _ => null };
        }
        catch (System.Security.Cryptography.CryptographicException) { return null; }
    }
    public void Write(HttpContext context, bool allow)
    {
        var payload = $"{policy.Version}|{(allow ? "allow" : "deny")}|{clock.GetUtcNow().UtcDateTime.Ticks}";
        context.Response.Cookies.Append(CookieName, protector.Protect(payload), new CookieOptions
        {
            Secure = true, HttpOnly = true, SameSite = SameSiteMode.Strict, IsEssential = true,
            Path = "/", MaxAge = TimeSpan.FromDays(180)
        });
        if (!allow)
        {
            foreach (var key in context.Request.Cookies.Keys.Where(x => x == "_ga" || x.StartsWith("_ga_", StringComparison.Ordinal) || x == "_gid" || x.StartsWith("_gat", StringComparison.Ordinal)))
            {
                context.Response.Cookies.Delete(key, new CookieOptions { Secure = true, Path = "/" });
                // GA may use a parent-domain cookie. Never derive domains from user-controlled configuration.
                var host = context.Request.Host.Host;
                if (Uri.CheckHostName(host) == UriHostNameType.Dns)
                {
                    var parts = host.Split('.');
                    for (var i = 0; i < parts.Length - 1; i++)
                    {
                        context.Response.Cookies.Delete(key, new CookieOptions { Secure = true, Path = "/", Domain = "." + string.Join('.', parts.Skip(i)) });
                    }
                }
            }
        }
    }
}
