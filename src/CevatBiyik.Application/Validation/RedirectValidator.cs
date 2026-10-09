using CevatBiyik.Application.DTOs;
using FluentValidation;
namespace CevatBiyik.Application.Validation;

public static class RedirectPaths
{
    private static readonly string[] ProtectedRoots = ["admin", "hata", "uploads", "app_data", "css", "js", "lib", "images", "sitemaps", "robots.txt", "sitemap.xml", "favicon.ico"];
    public static string Normalize(string path) => path.ToLowerInvariant();
    public static bool IsSafe(string? path)
    {
        if (string.IsNullOrEmpty(path) || path.Length > 200 || path[0] != '/' ||
            path.Any(c => !char.IsAsciiLetterOrDigit(c) && c is not '/' and not '-' and not '_' and not '.'))
        {
            return false;
        }

        if (path == "/")
        {
            return true;
        }

        var segments = path[1..].Split('/');
        return segments.All(x => x.Length > 0 && x is not "." and not ".." && x[0] != '.') &&
            !ProtectedRoots.Contains(segments[0], StringComparer.OrdinalIgnoreCase);
    }
    public static bool IsSource(string? path) => IsSafe(path) && !PageSeoValidator.Paths.Contains(Normalize(path!));
}
public sealed class RedirectValidator : AbstractValidator<RedirectInput>
{
    public RedirectValidator()
    {
        RuleFor(x => x.SourcePath).Must(RedirectPaths.IsSource).WithMessage("Kaynak, korunan veya mevcut sabit rota olmayan yerel bir yol olmalıdır.");
        RuleFor(x => x.DestinationPath).Must(RedirectPaths.IsSafe).WithMessage("Hedef yalnızca site içindeki temiz bir public yol olabilir.");
        RuleFor(x => x).Must(x => !string.Equals(x.SourcePath, x.DestinationPath, StringComparison.OrdinalIgnoreCase)).WithMessage("Kaynak ve hedef aynı olamaz.");
    }
}
