using CevatBiyik.Application.DTOs;

namespace CevatBiyik.Web.Models;

public static class ContactFormPolicy
{
    public static ContactViewModel Create(CreateContactMessageDto form, IWebHostEnvironment environment, IConfiguration configuration)
    {
        var notice = configuration["Site:ContactNotice"] ?? "CONTENT_REQUIRED";
        var approvedNotice = !string.IsNullOrWhiteSpace(notice) && !notice.Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase);
        var enabled = !environment.IsProduction() || (configuration.GetValue<bool>("Site:ContactFormEnabled") && approvedNotice);
        return new(form, enabled, notice);
    }
}
