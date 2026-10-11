using CevatBiyik.Application.Validation;

namespace CevatBiyik.UnitTests;

public sealed class ExpandedHtmlTests
{
    [Theory]
    [InlineData("<h4 class=\"content-align-center\">TEST_ONLY title</h4>")]
    [InlineData("<p><s>Old</s><del>Deleted</del>H<sub>2</sub>O x<sup>2</sup></p>")]
    public void ExtendedSemanticFormats_AreSafeAndRetainVisibleText(string html)
    {
        Assert.True(RichText.IsSafe(html));
        Assert.Contains("<", RichText.Render(html), StringComparison.Ordinal);
        Assert.DoesNotContain("<", RichText.ToPlainText(html), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("<h4 onclick=\"alert(1)\">Heading</h4>")]
    [InlineData("<sup style=\"position:fixed\">Text</sup>")]
    public void NewFormats_DoNotEnableAttributesOrCss(string html) => Assert.False(RichText.IsSafe(html));
}
