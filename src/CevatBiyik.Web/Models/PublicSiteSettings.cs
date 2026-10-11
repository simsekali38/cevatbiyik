namespace CevatBiyik.Web.Models;

public sealed class PublicSiteSettings(IReadOnlyDictionary<string, string> values)
{
    public string Name => Get("Site.Name", "Av. Cevat Bıyık");
    public string Description => Get("Site.Description", "Av. Cevat Bıyık — makaleler, çalışma alanları ve iletişim.");
    public string Email => Get("Contact.Email");
    public string Phone => Get("Contact.Phone");
    public string Address => Get("Contact.Address");
    public string WorkingHours => Get("Contact.WorkingHours");

    private string Get(string key, string fallback = "CONTENT_REQUIRED") =>
        values.TryGetValue(key, out var value) && CevatBiyik.Application.Validation.ContentPublication.Complete(value) ? value : fallback;

    public static PublicSiteSettings Empty { get; } = new(new Dictionary<string, string>());
}
