using CevatBiyik.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CevatBiyik.Infrastructure.Email;

// TLS is mandatory; recipient/header values never come from a public form.
public sealed class SmtpContactEmailSender(SmtpNotificationOptions options) : IContactEmailSender
{
    public static MimeMessage CreateMessage(SmtpNotificationOptions options, long id)
    {
        options.Validate();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        var message = new MimeMessage
        {
            Subject = "Yeni iletişim mesajı",
            Body = new TextPart("plain")
            {
                Text = $"Yeni iletişim mesajı kaydedildi. Kayıt numarası: {id}. Ayrıntılar için yönetim panelindeki iletişim mesajlarını açın."
            }
        };
        message.From.Add(MailboxAddress.Parse(options.From));
        message.To.Add(MailboxAddress.Parse(options.Recipient));
        return message;
    }
    public async Task SendAsync(long messageId, CancellationToken cancellationToken)
    {
        if (!options.Enabled) { throw new InvalidOperationException("SMTP notifications are disabled."); }
        cancellationToken.ThrowIfCancellationRequested();
        using var message = CreateMessage(options, messageId);
        using var client = new SmtpClient { Timeout = 30000 };
        var security = options.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
        await client.ConnectAsync(options.Host, options.Port, security, cancellationToken);
        await client.AuthenticateAsync(options.UserName, options.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        // SMTP accepted the message; a QUIT failure must not turn delivery into a retry.
        try { await client.DisconnectAsync(true, cancellationToken); }
        catch (Exception ex) when (ex is IOException or MailKit.ProtocolException or SmtpCommandException or OperationCanceledException) { }
    }
}
