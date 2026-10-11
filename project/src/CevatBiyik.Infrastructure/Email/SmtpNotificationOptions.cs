using System.Net.Mail;
using CevatBiyik.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CevatBiyik.Infrastructure.Email;

// Deliberately not a record: ToString must never print credentials.
public sealed class SmtpNotificationOptions : IContactNotificationPolicy
{
    public bool Enabled { get; init; }
    public string Host { get; init; } = "";
    public int Port { get; init; } = 587;
    public string UserName { get; init; } = "";
    public string Password { get; init; } = "";
    public string From { get; init; } = "";
    public string Recipient { get; init; } = "";
    public static SmtpNotificationOptions Read(IConfiguration config) => new()
    {
        Enabled = config.GetValue<bool>("Smtp:Enabled"), Host = config["Smtp:Host"] ?? "",
        Port = config.GetValue<int?>("Smtp:Port") ?? 587, UserName = config["Smtp:UserName"] ?? "",
        Password = config["Smtp:Password"] ?? "", From = config["Smtp:From"] ?? "", Recipient = config["Smtp:Recipient"] ?? ""
    };
    public void Validate()
    {
        if (!Enabled) { return; }
        if (Uri.CheckHostName(Host) == UriHostNameType.Unknown || Host.Length > 253 || Port != 587 ||
            string.IsNullOrWhiteSpace(UserName) || UserName.Length > 254 || UserName.Any(char.IsControl) ||
            string.IsNullOrWhiteSpace(Password) || Password.Length > 1024 ||
            !Mailbox(From) || !Mailbox(Recipient))
        {
            throw new InvalidOperationException("SMTP configuration is invalid. Use authenticated STARTTLS on port 587 and single mailboxes.");
        }
    }
    private static bool Mailbox(string value) => value.Length <= 254 && !value.Any(char.IsControl) &&
        MailAddress.TryCreate(value, out var address) && address.Address == value && !value.Contains("CONTENT_REQUIRED", StringComparison.OrdinalIgnoreCase);
}
