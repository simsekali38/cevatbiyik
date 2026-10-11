using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Controllers;

/// <summary>
/// Merkezi hata sayfaları (exception handler ve status code re-execute hedefi).
/// Hata sayfaları indexlenmez ve önbelleğe alınmaz.
/// </summary>
[Route("hata")]
public sealed class ErrorController : Controller
{
    // Status-code re-execution preserves the original verb, including contact POST failures.
    [Route("{statusCode:int?}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Index([FromRoute] int? statusCode)
    {
        // Malformed multipart forms can fail the value-provider factory before parameter binding.
        // The re-execution route remains trusted even when the nullable action parameter is unset.
        if (statusCode is null && int.TryParse(RouteData.Values["statusCode"]?.ToString(), out var routeStatus))
        {
            statusCode = routeStatus;
        }
        int code = statusCode is >= 400 and <= 599
            ? statusCode.Value
            : (statusCode is null ? StatusCodes.Status500InternalServerError : StatusCodes.Status404NotFound);

        Response.StatusCode = code;
        Response.Headers["X-Robots-Tag"] = "noindex, nofollow";

        return View(code);
    }
}
