using System.Net;
using System.Net.Mail;
using System.Text;
using CevatBiyik.Application.Interfaces;

namespace CevatBiyik.Infrastructure.Email;

// Narrow legacy SMTP adapter; TLS is mandatory, recipient/header values never come from a public form.
public sealed class SmtpContactEmailSender(SmtpNotificationOptions options) : IContactEmailSender
{
    public static MailMessage CreateMessage(SmtpNotificationOptions options, long id)
    {
        options.Validate();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        return new MailMessage(options.From, options.Recipient)
        {
            Subject = "Yeni iletişim mesajı",
            Body = $"Yeni iletişim mesajı kaydedildi. Kayıt numarası: {id}. Ayrıntılar için yönetim panelindeki iletişim mesajlarını açın.",
            IsBodyHtml = false, BodyEncoding = Encoding.UTF8, SubjectEncoding = Encoding.UTF8
        };
    }
    public async Task SendAsync(long messageId, CancellationToken cancellationToken)
    {
        if (!options.Enabled) { throw new InvalidOperationException("SMTP notifications are disabled."); }
        using var message = CreateMessage(options, messageId);
        using var client = new SmtpClient(options.Host, options.Port)
        {
            EnableSsl = true, UseDefaultCredentials = false,
            Credentials = new NetworkCredential(options.UserName, options.Password), DeliveryMethod = SmtpDeliveryMethod.Network
        };
        await client.SendMailAsync(message, cancellationToken);
    }
}
