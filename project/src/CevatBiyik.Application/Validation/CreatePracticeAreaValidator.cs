using CevatBiyik.Application.DTOs;
using FluentValidation;

namespace CevatBiyik.Application.Validation;

public sealed class CreatePracticeAreaValidator : AbstractValidator<CreatePracticeAreaDto>
{
    public CreatePracticeAreaValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).Must(PlainText.IsSafe);
        RuleFor(x => x.ShortDescription).MaximumLength(1000).Must(PlainText.IsSafe);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(100000).Must(RichText.IsSafe).WithMessage("İçerikte yalnızca güvenli başlık, paragraf, liste ve bağlantılar kullanılabilir.").Must(value => RichText.Fits(value, 100000));
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Seo).NotNull().SetValidator(new SeoValidator());
        RuleFor(x => x).Must(x => !x.IsActive || !string.Concat(x.Name, x.ShortDescription, RichText.ToPlainText(x.Content))
            .Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase))
            .WithMessage("CONTENT_REQUIRED içeriği yayımlanamaz.");
    }
}
