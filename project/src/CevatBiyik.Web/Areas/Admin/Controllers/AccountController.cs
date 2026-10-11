using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Area("Admin"), Route("admin"), AutoValidateAntiforgeryToken, RequestSizeLimit(16384)]
public sealed class AccountController(IAdminAuthenticationService authentication) : Controller
{
    [AllowAnonymous, HttpGet("login")]
    public IActionResult Login(string? returnUrl = null) => View(new LoginModel { ReturnUrl = returnUrl });

    [AllowAnonymous, HttpPost("login"), ValidateAntiForgeryToken, EnableRateLimiting("admin-login")]
    public async Task<IActionResult> Login([Bind("UserName,Password,ReturnUrl")] LoginModel input, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid && await authentication.SignInAsync(input.UserName.Trim(), input.Password, cancellationToken))
        {
            return LocalRedirect(Url.IsLocalUrl(input.ReturnUrl) ? input.ReturnUrl! : "/admin");
        }

        ModelState.AddModelError(string.Empty, "Giriş yapılamadı. Bilgilerinizi kontrol edin veya daha sonra tekrar deneyin.");
        input.Password = string.Empty;
        ModelState.Remove(nameof(LoginModel.Password));
        return View(input);
    }

    [Authorize(Roles = "Admin"), HttpPost("logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await authentication.SignOutAsync(cancellationToken);
        return Redirect("/admin/login");
    }

    [AllowAnonymous, HttpGet("access-denied")]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }
}
