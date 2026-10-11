namespace CevatBiyik.Web.Models;

public static class AdminLabels
{
    public static string Setting(string key) => key switch
    {
        "Site.Name" => "Site adı", "Site.Description" => "Site açıklaması",
        "Contact.Name" => "İletişim adı", "Contact.Email" => "E-posta", "Contact.Phone" => "Telefon",
        "Contact.Address" => "Adres", "Contact.WorkingHours" => "Çalışma saatleri", "Contact.WhatsApp" => "WhatsApp", _ => key
    };
}
