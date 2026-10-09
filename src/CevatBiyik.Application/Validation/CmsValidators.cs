using CevatBiyik.Application.DTOs;
using FluentValidation;
namespace CevatBiyik.Application.Validation;
public sealed class TaxonomyValidator : AbstractValidator<TaxonomyInput>
{
    public TaxonomyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150).Must(PlainText.IsSafe)
            .Must(x => x is not null && x.Any(char.IsLetterOrDigit)).WithMessage("Ad en az bir harf veya rakam içermelidir.");
        RuleFor(x => x).Must(x => !x.IsActive || !(x.Name ?? "").Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase));
    }
}
public sealed class SeoValidator : AbstractValidator<SeoInput>
{
    public SeoValidator()
    {
        RuleFor(x => x.MetaTitle).MaximumLength(120).Must(PlainText.IsSafe);
        RuleFor(x => x.MetaDescription).MaximumLength(320).Must(PlainText.IsSafe);
        RuleFor(x => x.Keywords).MaximumLength(300).Must(PlainText.IsSafe);
        RuleFor(x => x.OpenGraphTitle).MaximumLength(120).Must(PlainText.IsSafe);
        RuleFor(x => x.OpenGraphDescription).MaximumLength(320).Must(PlainText.IsSafe);
        RuleFor(x => x.OpenGraphMediaId).GreaterThan(0).When(x => x.OpenGraphMediaId.HasValue);
        RuleFor(x => x).Must(x => new[] { x.MetaTitle, x.MetaDescription, x.Keywords, x.OpenGraphTitle, x.OpenGraphDescription }
            .All(v => v is null || !v.Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase)));
    }
}
public sealed class PageSeoValidator : AbstractValidator<PageSeoInput>
{
    public static IReadOnlyList<string> Paths { get; } = new[] { "/", "/hakkimda", "/iletisim", "/makaleler", "/uzmanlik-alanlari" };
    public PageSeoValidator()
    {
        RuleFor(x => x.Path).Must(Paths.Contains);
        RuleFor(x => x.Seo).NotNull().SetValidator(new SeoValidator());
    }
}
