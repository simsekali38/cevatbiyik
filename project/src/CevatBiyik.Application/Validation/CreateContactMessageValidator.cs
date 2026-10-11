using CevatBiyik.Application.DTOs;
using FluentValidation;

namespace CevatBiyik.Application.Validation;

public sealed class CreateContactMessageValidator : AbstractValidator<CreateContactMessageDto>
{
    public CreateContactMessageValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150).Must(PlainText.IsSafe);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(254).EmailAddress().Must(PlainText.IsSafe)
            .Must(x => x is null || !x.Any(char.IsWhiteSpace))
            .Must(x => x is not null && System.Net.Mail.MailAddress.TryCreate(x, out var address) && address.Address == x);
        RuleFor(x => x.Phone).MaximumLength(30).Must(PlainText.IsSafe);
        RuleFor(x => x.Subject).MaximumLength(250).Must(PlainText.IsSafe);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(5000).Must(PlainText.IsSafe);
        RuleFor(x => x.Consent).Equal(true).WithMessage("İletişim izni gereklidir.");
    }
}
