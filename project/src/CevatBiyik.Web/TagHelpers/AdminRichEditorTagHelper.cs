using CevatBiyik.Application.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CevatBiyik.Web.TagHelpers;

[HtmlTargetElement("textarea", Attributes = "data-rich-editor,asp-for")]
public sealed class AdminRichEditorTagHelper : TagHelper
{
    [HtmlAttributeName("asp-for")] public ModelExpression For { get; set; } = null!;
    [ViewContext, HtmlAttributeNotBound] public ViewContext ViewContext { get; set; } = null!;
    public override int Order => 100;
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var name = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
        var value = ViewContext.ViewData.ModelState.TryGetValue(name, out var entry)
            ? entry.AttemptedValue ?? For.Model?.ToString() ?? string.Empty : For.Model?.ToString() ?? string.Empty;
        // Clipboard parsing must never receive untrusted stored/failed POST HTML.
        var safe = value.Length > 100000 ? string.Empty : RichText.HasMarkup(value) ? RichText.Normalize(value) : value;
        output.Content.SetContent(safe);
    }
}
