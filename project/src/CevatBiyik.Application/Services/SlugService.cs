using System.Globalization;
using System.Text;
using CevatBiyik.Application.Exceptions;
using CevatBiyik.Application.Interfaces;

namespace CevatBiyik.Application.Services;

public sealed class SlugService : ISlugService
{
    public string Generate(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var normalized = text.Replace('ı', 'i').Replace('İ', 'I').Normalize(NormalizationForm.FormD);
        var result = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var lower = char.ToLowerInvariant(c);
            if (lower is >= 'a' and <= 'z' or >= '0' and <= '9')
            {
                result.Append(lower);
            }
            else if (result.Length > 0 && result[^1] != '-')
            {
                result.Append('-');
            }
        }

        var slug = result.ToString().Trim('-');
        if (slug.Length == 0)
        {
            throw new ArgumentException("Başlık URL için en az bir Latin harfi veya rakam içermelidir.", nameof(text));
        }

        return slug[..Math.Min(slug.Length, 200)].TrimEnd('-');
    }

    public async Task<string> GenerateUniqueAsync(string text, Func<string, CancellationToken, Task<bool>> exists,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exists);
        var root = Generate(text);
        for (var attempt = 1; attempt <= 1000; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var suffix = attempt == 1 ? string.Empty : "-" + attempt.ToString(CultureInfo.InvariantCulture);
            var candidate = root[..Math.Min(root.Length, 200 - suffix.Length)].TrimEnd('-') + suffix;
            if (!await exists(candidate, cancellationToken))
            {
                return candidate;
            }
        }

        throw new ConflictException("Benzersiz URL oluşturulamadı; farklı bir başlık kullanın.");
    }
}
