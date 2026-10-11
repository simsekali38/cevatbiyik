using System.Globalization;
using System.Text.RegularExpressions;

namespace CevatBiyik.UnitTests;

public sealed class PublicDesignTests
{
    private static readonly int[] ChannelOffsets = [0, 2, 4];
    [Theory]
    [InlineData("brand-navy", "surface-paper", 4.5)]
    [InlineData("brand-navy", "surface-white", 4.5)]
    [InlineData("brand-gold-ink", "surface-white", 4.5)]
    [InlineData("brand-gold-ink", "surface-paper", 4.5)]
    [InlineData("text-muted", "surface-paper", 4.5)]
    [InlineData("text-muted", "surface-white", 4.5)]
    [InlineData("brand-gold", "brand-navy", 4.5)]
    [InlineData("surface-paper", "brand-navy", 4.5)]
    [InlineData("control-border", "surface-white", 3.0)]
    public void PublicTheme_ContrastMeetsBaseline(string foreground, string background, double minimum)
    {
        var css = File.ReadAllText(Path.Combine(RepoRoot.Find(), "src/CevatBiyik.Web/wwwroot/css/site.css"));
        double Luminance(string token)
        {
            var value = Regex.Matches(css, "--" + Regex.Escape(token) + @":\s*#([0-9A-Fa-f]{6})").Last().Groups[1].Value;
            var channels = ChannelOffsets.Select(offset => int.Parse(value.AsSpan(offset, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture) / 255d)
                .Select(channel => channel <= .04045 ? channel / 12.92 : Math.Pow((channel + .055) / 1.055, 2.4)).ToArray();
            return channels[0] * .2126 + channels[1] * .7152 + channels[2] * .0722;
        }
        var first = Luminance(foreground); var second = Luminance(background);
        var ratio = (Math.Max(first, second) + .05) / (Math.Min(first, second) + .05);
        Assert.True(ratio >= minimum, $"{foreground}/{background}: {ratio:F2}:1 < {minimum}:1");
    }

    [Fact]
    public void PublicBrandAsset_ContainsNoSourcePhotosOrExternalReferences()
    {
        var root = Path.Combine(RepoRoot.Find(), "src/CevatBiyik.Web/wwwroot");
        Assert.Empty(Directory.GetFiles(root, "20261008_*.jpg", SearchOption.AllDirectories));
        var svg = File.ReadAllText(Path.Combine(root, "images/brand/cevat-biyik-emblem.svg"));
        Assert.DoesNotContain("<script", svg, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("<image", svg, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("href=", svg, StringComparison.OrdinalIgnoreCase);
    }
}
