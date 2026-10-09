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

builder.Services.AddControllersWithViews(options => options.Conventions.Add(new AdminAreaConvention()));
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization(options => options.AddPolicy(AdminAccess.Policy,
    policy => policy.RequireAuthenticatedUser().RequireRole(AdminAccess.Role)));
builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromMinutes(5));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__Host-CevatBiyik.Admin";
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
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("admin-login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
    options.AddPolicy("contact", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
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

// Hata yönetimi baseline'ı: production'da detaylı hata sayfası YOK.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/hata");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/hata/{0}");

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/admin"))
    {
        context.Response.Headers["X-Robots-Tag"] = "noindex, nofollow";
        context.Response.Headers.CacheControl = "no-store";
    }
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; font-src 'self'; object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'";
    await next(context);
});
app.UseStaticFiles();

app.UseRouting();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

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
