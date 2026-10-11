using CevatBiyik.Application.DTOs;
using FluentValidation;

namespace CevatBiyik.Application.Validation;

public abstract class ArticleValidator<T> : AbstractValidator<T> where T : CreateArticleDto
{
    protected ArticleValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250).Must(PlainText.IsSafe);
        RuleFor(x => x.Excerpt).MaximumLength(1000).Must(PlainText.IsSafe);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(100000).Must(RichText.IsSafe).WithMessage("İçerikte yalnızca güvenli başlık, paragraf, liste ve bağlantılar kullanılabilir.").Must(value => RichText.Fits(value, 100000));
        RuleFor(x => x.PracticeAreaId).GreaterThan(0).When(x => x.PracticeAreaId.HasValue);
        RuleFor(x => x.CategoryId).GreaterThan(0).When(x => x.CategoryId.HasValue);
        RuleFor(x => x.CoverMediaId).GreaterThan(0).When(x => x.CoverMediaId.HasValue);
        RuleFor(x => x.Seo).NotNull().SetValidator(new SeoValidator());
        RuleFor(x => x.PublishedAt).NotNull().When(x => x.IsActive);
        RuleFor(x => x.PublishedAt).Must(x => !x.HasValue || x.Value.Kind == DateTimeKind.Utc)
            .WithMessage("Yayın tarihi UTC olmalıdır.");
        RuleFor(x => x).Must(x => !x.IsActive || !ContainsPlaceholder(x))
            .WithMessage("CONTENT_REQUIRED içeriği yayımlanamaz.");
    }

    private static bool ContainsPlaceholder(T x) =>
        string.Concat(x.Title, x.Excerpt, RichText.ToPlainText(x.Content)).Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase);
}

public sealed class CreateArticleValidator : ArticleValidator<CreateArticleDto>;
public sealed class UpdateArticleValidator : ArticleValidator<UpdateArticleDto>;
