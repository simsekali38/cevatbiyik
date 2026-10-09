using System.Text;
using System.Text.RegularExpressions;
using FluentValidation;

namespace CevatBiyik.Infrastructure.Media;
public static partial class PdfPolicy
{
    private static readonly HashSet<string> Forbidden = new(StringComparer.Ordinal)
    { "JavaScript", "JS", "AA", "OpenAction", "Launch", "EmbeddedFile", "EmbeddedFiles", "Filespec", "RichMedia", "AcroForm", "XFA", "Encrypt", "ObjStm", "URI", "GoToR", "GoToE", "SubmitForm", "ImportData", "Rendition", "Movie", "Sound", "3D", "Collection" };
    [GeneratedRegex(@"/([^\s/<>\[\](){}%]+)", RegexOptions.CultureInvariant)]
    private static partial Regex Names();
    [GeneratedRegex(@"#([0-9a-fA-F]{2})", RegexOptions.CultureInvariant)]
    private static partial Regex Escapes();
    [GeneratedRegex(@"startxref\s+([0-9]{1,10})\s+%%EOF\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex Xref();
    [GeneratedRegex(@"\A%PDF-(?:1\.[0-7]|2\.0)[\r\n]", RegexOptions.CultureInvariant)]
    private static partial Regex Header();
    public static void Validate(byte[] bytes)
    {
        var text = Encoding.Latin1.GetString(bytes);
        if (!Header().IsMatch(text)
            || !text.TrimEnd().EndsWith("%%EOF", StringComparison.Ordinal) || !text.Contains("startxref", StringComparison.Ordinal)
            || !text.Contains("/Catalog", StringComparison.Ordinal) || !text.Contains("/Pages", StringComparison.Ordinal))
        {
            throw new ValidationException("Geçerli, şifresiz bir PDF yükleyin.");
        }

        var xref = Xref().Match(text);
        if (!xref.Success || !long.TryParse(xref.Groups[1].Value, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var offset)
            || offset < 8 || offset > bytes.Length - 4 || !text.AsSpan((int)offset).StartsWith("xref", StringComparison.Ordinal)
            || !text.Contains("trailer", StringComparison.Ordinal))
        {
            throw new ValidationException("PDF çapraz başvuru tablosu geçersiz. Klasik xref içeren PDF olarak dışa aktarın.");
        }
        foreach (Match name in Names().Matches(text))
        {
            var decoded = Escapes().Replace(name.Groups[1].Value, x => ((char)Convert.ToInt32(x.Groups[1].Value, 16)).ToString());
            if (Forbidden.Contains(decoded))
            {
                throw new ValidationException("PDF aktif içerik, ek dosya, form, şifreleme veya sıkıştırılmış nesne içeremez.");
            }
        }
        // This conservative content gate is not a full PDF parser or a replacement for antivirus.
    }
}
