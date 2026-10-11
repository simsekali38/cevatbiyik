using CevatBiyik.Application.DTOs;
using FluentValidation;

namespace CevatBiyik.Application.Validation;

public sealed class UpdateSiteSettingValidator : AbstractValidator<UpdateSiteSettingDto>
{
    public UpdateSiteSettingValidator()
    {
        RuleFor(x => x.Key).NotEmpty().MaximumLength(150).Must(x => PublicSettingKeys.All.Contains(x))
            .WithMessage("Yalnızca tanımlı, herkese açık site ayarları saklanabilir.");
        RuleFor(x => x.Value).NotEmpty().MaximumLength(4000).Must(PlainText.IsSafe);
        RuleFor(x => x.Value).EmailAddress().Must(x => x is null || !x.Any(char.IsWhiteSpace))
            .When(x => x.Key == "Contact.Email" && x.Value != "CONTENT_REQUIRED");
        RuleFor(x => x.Value).Must(value => long.TryParse(value, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var id) && id > 0)
            .When(x => x.Key == "Home.HeroMediaId" && x.IsActive)
            .WithMessage("Hero için medya kütüphanesindeki pozitif görsel ID'sini girin.");
        RuleFor(x => x).Must(x => !x.IsActive || !(x.Value ?? string.Empty)
            .Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase))
            .WithMessage("CONTENT_REQUIRED ayarı etkinleştirilemez.");
    }
}
