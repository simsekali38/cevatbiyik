using CevatBiyik.Application.DTOs;
using FluentValidation;
namespace CevatBiyik.Application.Validation;
public static class ContentPublication
{
    public static IReadOnlyDictionary<string, string> Pages { get; } = new Dictionary<string, string>(StringComparer.Ordinal)
    { ["/"] = "Ana sayfa giriş metni", ["/kvkk"] = "KVKK / Aydınlatma", ["/cerez-politikasi"] = "Çerez Politikası", ["/yasal-uyari"] = "Yasal Uyarı" };
    public static bool Complete(params string?[] texts) => texts.All(x => !string.IsNullOrWhiteSpace(RichText.ToPlainText(x)) && !RichText.ToPlainText(x).Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase));
}
public sealed class ProfileValidator : AbstractValidator<ProfileInput>
{
    public ProfileValidator()
    {
        RuleFor(x => x.Biography).NotEmpty().MaximumLength(5000).Must(RichText.IsSafe).WithMessage("İçerikte yalnızca güvenli biçimlendirme kullanılabilir.").Must(value => RichText.Fits(value, 5000)).WithMessage("Biçimlendirme dahil içerik karakter sınırını aşıyor.");
        RuleFor(x => x.Vision).NotEmpty().MaximumLength(2000).Must(RichText.IsSafe).WithMessage("İçerikte yalnızca güvenli biçimlendirme kullanılabilir.").Must(value => RichText.Fits(value, 2000)).WithMessage("Biçimlendirme dahil içerik karakter sınırını aşıyor.");
        RuleFor(x => x.BarAssociation).NotEmpty().MaximumLength(150).Must(PlainText.IsSafe);
        RuleFor(x => x).Must(x => !x.IsActive || x.IsReviewed).WithMessage("Yayımlamadan önce içerik inceleme onayı gerekir.");
        RuleFor(x => x).Must(x => !x.IsReviewed || ContentPublication.Complete(x.Biography, x.Vision, x.BarAssociation)).WithMessage("Eksik içerik onaylanamaz.");
    }
}
public sealed class PageContentValidator : AbstractValidator<PageContentInput>
{
    public PageContentValidator()
    {
        RuleFor(x => x.Path).Must(x => x is not null && ContentPublication.Pages.ContainsKey(x));
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200).Must(PlainText.IsSafe);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(12000).Must(RichText.IsSafe).WithMessage("İçerikte yalnızca güvenli biçimlendirme kullanılabilir.").Must(value => RichText.Fits(value, 12000)).WithMessage("Biçimlendirme dahil içerik karakter sınırını aşıyor.");
        RuleFor(x => x).Must(x => !x.IsActive || x.IsReviewed).WithMessage("Yayımlamadan önce içerik inceleme onayı gerekir.");
        RuleFor(x => x).Must(x => !x.IsReviewed || ContentPublication.Complete(x.Title, x.Body)).WithMessage("Eksik içerik onaylanamaz.");
    }
}
public sealed class FaqValidator : AbstractValidator<FaqInput>
{
    public FaqValidator()
    {
        RuleFor(x => x.Question).NotEmpty().MaximumLength(250).Must(PlainText.IsSafe);
        RuleFor(x => x.Answer).NotEmpty().MaximumLength(4000).Must(RichText.IsSafe).WithMessage("İçerikte yalnızca güvenli biçimlendirme kullanılabilir.").Must(value => RichText.Fits(value, 4000)).WithMessage("Biçimlendirme dahil içerik karakter sınırını aşıyor.");
        RuleFor(x => x.SortOrder).InclusiveBetween(0, 10000);
        RuleFor(x => x).Must(x => !x.IsActive || x.IsReviewed).WithMessage("Yayımlamadan önce içerik inceleme onayı gerekir.");
        RuleFor(x => x).Must(x => !x.IsReviewed || ContentPublication.Complete(x.Question, x.Answer)).WithMessage("Eksik içerik onaylanamaz.");
    }
}
