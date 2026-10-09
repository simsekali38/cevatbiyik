using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CevatBiyik.Web.Filters;

/// <summary>Only applied to public content controllers, never to error re-execution.</summary>
public sealed class PublicSiteFilter(ISettingService settings) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is Controller controller)
        {
            var values = await settings.ListPublicAsync(context.HttpContext.RequestAborted);
            controller.ViewData["SiteSettings"] = new PublicSiteSettings(values.ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal));
        }

        await next();
    }
}
