using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CevatBiyik.Web.TagHelpers;

[HtmlTargetElement("a", Attributes = "asp-action,asp-route-id")]
[HtmlTargetElement("a", Attributes = "data-edit-action")]
public sealed class AdminActionLinkTagHelper : TagHelper
{
    [ViewContext, HtmlAttributeNotBound] public ViewContext ViewContext { get; set; } = null!;
    public override int Order => 200;
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext.RouteData.Values["area"]?.ToString() != "Admin") { return; }
        var action = context.AllAttributes["asp-action"]?.Value?.ToString();
        if (action != "Edit" && !context.AllAttributes.ContainsName("data-edit-action")) { return; }
        if (context.AllAttributes.ContainsName("asp-route-choicesPage")) { return; }
        var classes = output.Attributes["class"]?.Value?.ToString();
        output.Attributes.SetAttribute("class", classes + " admin-action admin-action-edit");
        output.Content.SetHtmlContent("<svg viewBox=\"0 0 24 24\" aria-hidden=\"true\" focusable=\"false\"><path d=\"m15 4 5 5M4 20l5-1L20 8a3.5 3.5 0 0 0-5-5L4 14Z\" /></svg><span>Düzenle</span>");
    }
}
