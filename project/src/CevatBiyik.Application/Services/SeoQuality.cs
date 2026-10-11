using CevatBiyik.Application.DTOs;
namespace CevatBiyik.Application.Services;

public sealed record SeoIssue(string Code, string Message);
public static class SeoQuality
{
    // Editorial suggestions, not search-engine hard limits or ranking promises.
    public static IReadOnlyList<SeoIssue> Check(SeoInput seo, string? fallbackTitle = null)
    {
        var issues = new List<SeoIssue>();
        if (!new CevatBiyik.Application.Validation.SeoValidator().Validate(seo).IsValid)
        {
            issues.Add(new("metadata-invalid", "SEO alanlarında geçersiz metin bulunuyor; public görünüm varsayılan metinleri kullanır."));
        }
        var title = string.IsNullOrWhiteSpace(seo.MetaTitle) ? fallbackTitle?.Trim() : seo.MetaTitle.Trim();
        if (string.IsNullOrWhiteSpace(seo.MetaTitle))
        {
            issues.Add(new("title-fallback", "Meta başlık boş; sayfa başlığı ve site adı kullanılacak."));
        }

        if (string.IsNullOrWhiteSpace(title) || title.Length < 30)
        {
            issues.Add(new("title-short", "Başlık kısa; 30–60 karakter aralığını içerik bağlamında değerlendirin."));
        }
        else if (title.Length > 60)
        {
            issues.Add(new("title-long", "Başlık uzun; arama sonucunda kesilebilir."));
        }

        var description = seo.MetaDescription?.Trim();
        if (string.IsNullOrWhiteSpace(description))
        {
            issues.Add(new("description-missing", "Meta açıklama boş; sayfanın varsayılan açıklaması kullanılacak."));
        }
        else if (description.Length < 70 || description.Length > 160)
        {
            issues.Add(new("description-length", "Açıklama için 70–160 karakter aralığını değerlendirin."));
        }

        if (string.IsNullOrWhiteSpace(seo.OpenGraphTitle) || string.IsNullOrWhiteSpace(seo.OpenGraphDescription))
        {
            issues.Add(new("og-fallback", "Paylaşım metinleri boşsa SEO başlık/açıklaması kullanılacak."));
        }

        if (!seo.OpenGraphMediaId.HasValue)
        {
            issues.Add(new("og-image-optional", "Paylaşım görseli seçilmedi; görsel isteğe bağlıdır."));
        }

        return issues;
    }
}
