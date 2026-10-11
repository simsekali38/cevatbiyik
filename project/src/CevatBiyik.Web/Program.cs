using System.Threading.RateLimiting;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Infrastructure;
using CevatBiyik.Infrastructure.Identity;
using CevatBiyik.Web.Filters;
using CevatBiyik.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;

var provisionAdmin = args.Contains("--provision-admin", StringComparer.Ordinal);
var builder = WebApplication.CreateBuilder(args.Where(x => x != "--provision-admin").ToArray());
builder.Services.AddHostingDataProtection(builder.Configuration, builder.Environment);

builder.Services.AddControllersWithViews(options =>
{
    options.Conventions.Add(new AdminAreaConvention());
    options.Conventions.Add(new PrivateDocumentsConvention(builder.Configuration));
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization(options => options.AddPolicy(AdminAccess.Policy,
    policy => policy.RequireAuthenticatedUser().RequireRole(AdminAccess.Role)));
builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMinutes(5));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__Host-CevatBiyik.Admin";
    options.Cookie.Path = "/";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.LoginPath = "/admin/login";
    options.AccessDeniedPath = "/admin/access-denied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = false;
    options.Events.OnValidatePrincipal = async context =>
    {
        await SecurityStampValidator.ValidatePrincipalAsync(context);
        if (context.Principal?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var users = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await users.GetUserAsync(context.Principal);
        if (user is null || !user.IsActive || !await users.IsInRoleAsync(user, AdminAccess.Role))
        {
            context.RejectPrincipal();
            await context.HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>().SignOutAsync();
        }
    };
});
builder.Services.AddScoped<PublicSiteFilter>();
builder.Services.AddScoped<CevatBiyik.Web.Analytics.AnalyticsPolicy>();
builder.Services.AddScoped<CevatBiyik.Web.Analytics.AnalyticsConsent>();
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
});
builder.Services.AddRateLimiter(options =>
{
    // Static files would otherwise short-circuit before endpoint limiters.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        context.Request.Path.StartsWithSegments("/uploads", StringComparison.OrdinalIgnoreCase) || Path.HasExtension(context.Request.Path.Value)
            ? RateLimitPartition.GetFixedWindowLimiter("static:" + (context.Connection.RemoteIpAddress?.MapToIPv6().ToString() ?? "unknown"),
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 120, Window = TimeSpan.FromMinutes(1), QueueLimit = 0, AutoReplenishment = true })
            : RateLimitPartition.GetNoLimiter("public"));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = (context, _) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "60";
        return ValueTask.CompletedTask;
    };
    options.AddPolicy("consent", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.MapToIPv6().ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
        { PermitLimit = 20, Window = TimeSpan.FromMinutes(1), QueueLimit = 0, AutoReplenishment = true }));
    options.AddPolicy("admin-login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.MapToIPv6().ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
    options.AddPolicy("contact", context => RateLimitPartition.GetSlidingWindowLimiter(
        context.Connection.RemoteIpAddress?.MapToIPv6().ToString() ?? "unknown", _ => new SlidingWindowRateLimiterOptions
        {
            SegmentsPerWindow = 6,
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
});

builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
        | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 1;
    options.RequireHeaderSymmetry = true;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    options.KnownProxies.Add(System.Net.IPAddress.Loopback);
    options.KnownProxies.Add(System.Net.IPAddress.IPv6Loopback);
    var proxies = builder.Configuration.GetSection("Security:TrustedProxies").GetChildren().ToArray();
    if (proxies.Length > 16) { throw new InvalidOperationException("At most 16 trusted proxy addresses are permitted."); }
    foreach (var proxy in proxies)
    {
        if (!System.Net.IPAddress.TryParse(proxy.Value, out var address) || address.Equals(System.Net.IPAddress.Any)
            || address.Equals(System.Net.IPAddress.IPv6Any))
        {
            throw new InvalidOperationException("Security:TrustedProxies must contain explicit IP addresses.");
        }
        options.KnownProxies.Add(address);
    }
});
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(180);
    // No preload/subdomain assertion until hosting and every affected hostname are verified.
    options.IncludeSubDomains = false;
    options.Preload = false;
});
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options => options.HttpsPort = 443);
}

// HTTPS compression is limited to public assets. Never compress HTML containing CSRF/session data.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = ["text/css", "text/javascript", "application/javascript", "image/svg+xml"];
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

var app = builder.Build();

if (provisionAdmin)
{
    await using var scope = app.Services.CreateAsyncScope();
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<CevatBiyik.Infrastructure.Persistence.AppDbContext>();
    await using var transaction = await db.Database.BeginTransactionAsync();
    await AdminProvisioner.ProvisionAsync(services.GetRequiredService<UserManager<ApplicationUser>>(),
        services.GetRequiredService<RoleManager<IdentityRole>>(), services.GetRequiredService<IAuditLogService>(), builder.Configuration);
    await transaction.CommitAsync();
    return;
}

app.UseForwardedHeaders();
app.UseMiddleware<SecurityHeadersMiddleware>();

// Hata yönetimi baseline'ı: production'da detaylı hata sayfası YOK.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/hata");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/hata/{0}");

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseMiddleware<ConsentCspMiddleware>();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/uploads", StringComparison.OrdinalIgnoreCase) &&
        !System.Text.RegularExpressions.Regex.IsMatch(context.Request.Path.Value ?? "", @"\A/uploads/images/[a-f0-9]{32}\.webp\z", System.Text.RegularExpressions.RegexOptions.CultureInvariant))
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }
    await next(context);
});
app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = CevatBiyik.Web.Performance.StaticAssetPolicy.PrepareResponse });

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CevatBiyik.Web.Seo.RedirectMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

/// <summary>
/// Integration testlerin (WebApplicationFactory) giriş noktasına erişebilmesi için.
/// </summary>
public partial class Program
{
}
