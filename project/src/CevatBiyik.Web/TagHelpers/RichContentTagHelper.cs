using CevatBiyik.Application.Validation;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CevatBiyik.Web.TagHelpers;

[HtmlTargetElement("rich-content", Attributes = "value")]
public sealed class RichContentTagHelper : TagHelper
{
    public string? Value { get; set; }
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div"; output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.RemoveAll("value");
        var existing = output.Attributes["class"]?.Value?.ToString();
        output.Attributes.SetAttribute("class", string.Join(" ", new[] { existing, RichText.HasMarkup(Value) ? "rich-content" : "plain-content" }.Where(x => !string.IsNullOrEmpty(x))));
        // Only policy-sanitized HTML or HTML-encoded legacy text reaches this controlled sink.
        output.Content.SetHtmlContent(RichText.Render(Value));
    }
}
