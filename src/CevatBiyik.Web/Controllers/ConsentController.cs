using CevatBiyik.Web.Analytics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CevatBiyik.Web.Controllers;

public sealed class ConsentController(AnalyticsPolicy policy, AnalyticsConsent consent) : Controller
{
    [HttpPost("/cerez-tercihi")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("consent")]
    [RequestSizeLimit(4096)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Save(string choice, string? returnUrl)
    {
        if (choice is not ("allow" or "deny")) { return BadRequest(); }
        consent.Write(HttpContext, choice == "allow" && await policy.AvailableAsync(HttpContext));
        // Do not carry form/search query strings into consent redirects.
        var target = returnUrl?.Split('?', '#')[0];
        return LocalRedirect(target is not null && Url.IsLocalUrl(target) && target.StartsWith('/') && !target.StartsWith("/admin", StringComparison.OrdinalIgnoreCase) ? target : "/");
    }
}
