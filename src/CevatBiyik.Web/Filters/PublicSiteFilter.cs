using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CevatBiyik.Web.Filters;

/// <summary>Only applied to public content controllers, never to error re-execution.</summary>
public sealed class PublicSiteFilter(ISettingService settings, IPageSeoService seo, IConfiguration configuration) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is Controller controller)
        {
            var values = await settings.ListPublicAsync(context.HttpContext.RequestAborted);
            controller.ViewData["SiteSettings"] = new PublicSiteSettings(values.ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal));
        }

        await next();
        if (context.Controller is Controller page && context.HttpContext.Response.StatusCode == 200)
        {
            var path = context.HttpContext.Request.Path.Value ?? "/";
            if (CevatBiyik.Application.Validation.PageSeoValidator.Paths.Contains(path) &&
                await seo.ForPathAsync(path, context.HttpContext.RequestAborted) is { } custom)
            {
                CevatBiyik.Web.Seo.SeoPresentation.Apply(page.ViewData, custom.Seo, custom.OpenGraphImageUrl);
            }

            if (path == "/iletisim" && page.ViewData["SiteSettings"] is PublicSiteSettings site)
            {
                page.ViewData["StructuredData"] = CevatBiyik.Web.Seo.SeoPresentation.LegalService(site, configuration);
            }
        }
    }
}
