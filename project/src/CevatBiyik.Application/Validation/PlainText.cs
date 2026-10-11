using System.Net;

namespace CevatBiyik.Application.Validation;

internal static class PlainText
{
    public static bool IsSafe(string? text)
    {
        if (text is null)
        {
            return true;
        }

        // Defense in depth. Output must still be encoded by Razor; this is not an HTML sanitizer.
        for (var i = 0; i < 5; i++)
        {
            if (text.Any(c => c is '<' or '>' || (char.IsControl(c) && c is not '\r' and not '\n' and not '\t')))
            {
                return false;
            }

            var decoded = WebUtility.HtmlDecode(text);
            if (decoded == text)
            {
                return true;
            }

            text = decoded;
        }

        return false;
    }
}
