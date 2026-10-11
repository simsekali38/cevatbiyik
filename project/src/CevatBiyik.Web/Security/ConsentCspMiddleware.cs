namespace CevatBiyik.Web.Security;

/// <summary>Consent-dependent database access stays inside the exception handler.</summary>
public sealed class ConsentCspMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var nonce = (string)context.Items["CspNonce"]!;
        // No external analytics origins on admin, errors, unknown consent or incomplete legal content.
        if (!context.Request.Path.StartsWithSegments("/admin") && !context.Request.Path.StartsWithSegments("/hata") &&
            !context.Request.Path.StartsWithSegments("/uploads") && !Path.HasExtension(context.Request.Path.Value))
        {
            var analytics = context.RequestServices.GetRequiredService<CevatBiyik.Web.Analytics.AnalyticsPolicy>();
            var consent = context.RequestServices.GetRequiredService<CevatBiyik.Web.Analytics.AnalyticsConsent>();
            if (analytics.Configured) { context.Response.Headers.CacheControl = "no-store"; }
            if (analytics.Configured && consent.Read(context) == true && await analytics.AvailableAsync(context))
            {
                context.Response.Headers.CacheControl = "no-store";
                context.Response.Headers["Content-Security-Policy"] = $"default-src 'self'; script-src 'self' 'nonce-{nonce}' https://www.googletagmanager.com; connect-src 'self' https://www.google-analytics.com https://region1.google-analytics.com https://analytics.google.com https://www.googletagmanager.com; style-src 'self'; img-src 'self' data: https://www.google-analytics.com https://www.googletagmanager.com; font-src 'self'; object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'; frame-src 'none'";
            }
        }
        await next(context);
    }
}
