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
        CancellationToken cancellationToken)
    {
        var model = ContactFormPolicy.Create(form, environment, configuration);
        if (environment.IsProduction() && await pages.PublishedAsync("/kvkk", cancellationToken) is null)
        {
            model = model with { CanSubmit = false };
        }

        if (!model.CanSubmit)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
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
        TempData["ContactSuccess"] = "Mesajınız kaydedildi. Teşekkür ederiz.";
        return RedirectToAction(nameof(HomeController.Contact), "Home");
    }
}
