using System.Security.Cryptography;

namespace CevatBiyik.Web.Security;

/// <summary>Protects redirects, errors, static files and other short-circuit responses.</summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
        context.Items["CspNonce"] = nonce;
        var isAdmin = context.Request.Path.StartsWithSegments("/admin", StringComparison.OrdinalIgnoreCase);
        var policy = $"default-src 'self'; script-src 'self' 'nonce-{nonce}'; connect-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self'; object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'; frame-src 'none'";
        context.Response.Headers["Content-Security-Policy"] = policy;
        context.Response.OnStarting(() =>
        {
            // ExceptionHandler can clear headers. Restore them at the final response boundary.
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=(), usb=()";
            if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
            {
                context.Response.Headers["Content-Security-Policy"] = policy;
            }
            if (!configuration.GetValue<bool>("Site:AllowIndexing") || isAdmin || context.Response.StatusCode >= 400)
            {
                context.Response.Headers["X-Robots-Tag"] = "noindex, nofollow";
            }
            if ((isAdmin || context.Response.StatusCode >= 400 || context.Response.ContentType?.StartsWith("text/html", StringComparison.OrdinalIgnoreCase) == true)
                && !context.Response.Headers.CacheControl.ToString().Contains("no-store", StringComparison.Ordinal))
            {
                context.Response.Headers.CacheControl = "no-store";
            }
            // Retain stricter download sandbox and consent-approved CSP supplied downstream.
            return Task.CompletedTask;
        });
        await next(context);
    }
}
