using System.Collections.Frozen;

namespace CevatBiyik.Application.Validation;

public static class PublicSettingKeys
{
    public static IReadOnlySet<string> All { get; } = new[]
    {
        "Site.Name", "Site.Description", "Contact.Name", "Contact.Email", "Contact.Phone",
        "Contact.Address", "Contact.WorkingHours", "Contact.WhatsApp", "Home.HeroMediaId"
    }.ToFrozenSet(StringComparer.Ordinal);
}
