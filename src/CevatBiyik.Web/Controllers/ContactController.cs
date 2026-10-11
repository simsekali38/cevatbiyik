using CevatBiyik.Application.DTOs;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Web.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CevatBiyik.Web.Controllers;

public sealed class ContactController(IContactService contacts, IValidator<CreateContactMessageDto> validator,
    IWebHostEnvironment environment, IConfiguration configuration, IPageContentService pages) : PublicController
{
    [HttpPost("/iletisim")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("contact")]
    [RequestSizeLimit(32768)]
    [RequestFormLimits(ValueCountLimit = 16, KeyLengthLimit = 128, ValueLengthLimit = 20000)]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Submit([Bind("Name,Email,Phone,Subject,Message,Consent", Prefix = "Form")] CreateContactMessageDto form,
        CancellationToken cancellationToken, [FromForm(Name = "Website")] string? website = null)
    {
        var model = ContactFormPolicy.Create(form, environment, configuration)
            with { Privacy = await pages.PublishedAsync("/kvkk", cancellationToken) };
        if (environment.IsProduction() && model.Privacy is null)
        {
            model = model with { CanSubmit = false };
        }

        if (!model.CanSubmit)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        if (!string.IsNullOrEmpty(website))
        {
            TempData["ContactSuccess"] = "Mesajınız kaydedildi. Teşekkür ederiz.";
            return RedirectToAction(nameof(HomeController.Contact), "Home");
        }
        var result = await validator.ValidateAsync(form, cancellationToken);
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("Form." + error.PropertyName, "Lütfen bu alana geçerli bir değer girin.");
        }

        if (!ModelState.IsValid)
        {
            SetPage("İletişim", "Av. Cevat Bıyık iletişim bilgileri ve iletişim formu.", "/iletisim", true);
            return View("~/Views/Home/Contact.cshtml", model);
        }

        await contacts.CreateAsync(form, cancellationToken);
        TempData["ContactRecorded"] = true;
        TempData["ContactSuccess"] = "Mesajınız kaydedildi. Teşekkür ederiz.";
        return RedirectToAction(nameof(HomeController.Contact), "Home");
    }
}
