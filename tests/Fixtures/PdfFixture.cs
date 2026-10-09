using System.Globalization;
using System.Text;

namespace CevatBiyik.Tests;
public static class PdfFixture
{
    public static byte[] Create(string extra = "")
    {
        var text = new StringBuilder("%PDF-1.4\n");
        var bodies = new[]
        {
            "<< /Type /Catalog /Pages 2 0 R " + extra + " >>",
            "<< /Type /Pages /Count 1 /Kids [3 0 R] >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << >> /Contents 4 0 R >>",
            "<< /Length 0 >>\nstream\n\nendstream"
        };
        var offsets = new List<int>();
        for (var i = 0; i < bodies.Length; i++)
        {
            offsets.Add(text.Length);
            text.Append(CultureInfo.InvariantCulture, $"{i + 1} 0 obj\n{bodies[i]}\nendobj\n");
        }
        var xref = text.Length;
        text.Append("xref\n0 5\n0000000000 65535 f \n");
        foreach (var offset in offsets)
        {
            text.Append(offset.ToString("D10", CultureInfo.InvariantCulture)).Append(" 00000 n \n");
        }

        text.Append(CultureInfo.InvariantCulture, $"trailer\n<< /Size 5 /Root 1 0 R >>\nstartxref\n{xref}\n%%EOF\n");
        return Encoding.ASCII.GetBytes(text.ToString());
    }
}
