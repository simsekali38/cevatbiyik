using System.Security.Claims;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CevatBiyik.Web.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = "Admin", Policy = "AdminOnly"), AutoValidateAntiforgeryToken, RequestSizeLimit(262144)]
public abstract class AdminControllerBase(IAdminOperation operations) : Controller
{
    protected async Task<IActionResult> SaveAsync(object input, Func<Task> mutation, SecurityEvent securityEvent,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("Edit", input);
        }

        try
        {
            await operations.ExecuteAsync(securityEvent, User.FindFirstValue(ClaimTypes.NameIdentifier)!, mutation, cancellationToken);
            TempData["AdminSuccess"] = "İşlem tamamlandı.";
            return RedirectToAction("Index");
        }
        catch (ValidationException exception)
        {
            foreach (var error in exception.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
        catch (ConflictException) { ModelState.AddModelError(string.Empty, "Bu kayıtla çakışan bir değer zaten var."); }
        catch (NotFoundException) { return NotFound(); }
        return View("Edit", input);
    }

    protected async Task<IActionResult> ChangeAsync(Func<Task> mutation, SecurityEvent securityEvent, CancellationToken cancellationToken)
    {
        try
        {
            await operations.ExecuteAsync(securityEvent, User.FindFirstValue(ClaimTypes.NameIdentifier)!, mutation, cancellationToken);
            TempData["AdminSuccess"] = "İşlem tamamlandı.";
            return RedirectToAction("Index");
        }
        catch (NotFoundException) { return NotFound(); }
    }
}
