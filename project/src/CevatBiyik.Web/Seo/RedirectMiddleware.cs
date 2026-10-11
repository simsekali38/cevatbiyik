using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Validation;
namespace CevatBiyik.Web.Seo;

public sealed class RedirectMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IRedirectService redirects)
    {
        var path = context.Request.Path.Value ?? "/";
        if ((HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method)) && RedirectPaths.IsSource(path) &&
            await redirects.ResolveAsync(path, context.RequestAborted) is { } target)
        {
            context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
            context.Response.Headers.Location = target;
            context.Response.Headers.CacheControl = "no-store";
            return;
        }
        await next(context);
    }
}
