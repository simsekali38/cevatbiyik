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
    public IActionResult Index(int? statusCode)
    {
        int code = statusCode is >= 400 and <= 599
            ? statusCode.Value
            : (statusCode is null ? StatusCodes.Status500InternalServerError : StatusCodes.Status404NotFound);

        Response.StatusCode = code;
        Response.Headers["X-Robots-Tag"] = "noindex, nofollow";

        return View(code);
    }
}
