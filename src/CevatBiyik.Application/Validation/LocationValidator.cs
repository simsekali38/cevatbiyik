using CevatBiyik.Application.DTOs;
using CevatBiyik.Domain.Entities;
using FluentValidation;
namespace CevatBiyik.Application.Validation;
public sealed class LocationValidator : AbstractValidator<LocationInput>
{
    public LocationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).Must(PlainText.IsSafe).Must(x => x is not null && !x.Any(char.IsControl));
        RuleFor(x => x.Name).Matches("[a-zA-Z0-9çğıöşüÇĞİÖŞÜ]").WithMessage("URL için Latin harfi veya rakam gerekir.");
        RuleFor(x => x.Kind).IsInEnum();
        RuleFor(x => x.ParentId).GreaterThan(0).When(x => x.ParentId.HasValue);
        RuleFor(x => x.Summary).NotEmpty().MaximumLength(500).Must(PlainText.IsSafe);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(12000).Must(PlainText.IsSafe);
        RuleFor(x => x.LocalDetails).NotEmpty().MaximumLength(4000).Must(PlainText.IsSafe);
        RuleFor(x => x.Seo).NotNull().SetValidator(new SeoValidator());
        RuleFor(x => x.PracticeAreaIds).NotNull().Must(x => x is not null && x.Length <= 20 && x.All(i => i > 0) && x.Distinct().Count() == x.Length);
        RuleFor(x => x.ArticleIds).NotNull().Must(x => x is not null && x.Length <= 20 && x.All(i => i > 0) && x.Distinct().Count() == x.Length);
        RuleFor(x => x).Must(x => !x.IsActive || x.IsReviewed).WithMessage("Yayın için insan incelemesi gerekir.");
        RuleFor(x => x).Must(x => !x.IsReviewed || ContentPublication.Complete(x.Name, x.Summary, x.Body, x.LocalDetails)).WithMessage("CONTENT_REQUIRED içeren içerik onaylanamaz.");
        RuleFor(x => x).Must(x => x.Kind != LocationKind.Province || (x.Name == "Kayseri" && x.ParentId is null)).WithMessage("Bu faz yalnızca Kayseri il hiyerarşisini destekler.");
    }
}
