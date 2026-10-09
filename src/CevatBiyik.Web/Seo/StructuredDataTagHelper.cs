using System.Text.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;
namespace CevatBiyik.Web.Seo;
[HtmlTargetElement("structured-data")]
public sealed class StructuredDataTagHelper : TagHelper
{
    public object? Model { get; set; }
    public string? Nonce { get; set; }
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (Model is null) { output.SuppressOutput(); return; }
        output.TagName = "script"; output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.Clear(); output.Attributes.SetAttribute("type", "application/ld+json");
        output.Attributes.SetAttribute("nonce", Nonce);
        // The default JSON encoder escapes HTML delimiters, including </script>. Never use UnsafeRelaxedJsonEscaping.
        output.Content.SetHtmlContent(JsonSerializer.Serialize(Model));
    }
}
