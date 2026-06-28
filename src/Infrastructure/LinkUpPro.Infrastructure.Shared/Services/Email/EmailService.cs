using LinkUpPro.Infrastructure.Shared.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LinkUpPro.Infrastructure.Shared.Services.Email;

// Implementación del servicio de correo usando MailKit/SMTP.
public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendActivationEmailAsync(string toEmail, string userName, string activationUrl)
    {
        var subject = "LinkUp Pro — Activa tu cuenta";
        var htmlBody = EmailTemplates.GetActivationTemplate(userName, activationUrl);
        await SendEmailAsync(toEmail, subject, htmlBody);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetUrl)
    {
        var subject = "LinkUp Pro — Restablecer contraseña";
        var htmlBody = EmailTemplates.GetPasswordResetTemplate(userName, resetUrl);
        await SendEmailAsync(toEmail, subject, htmlBody);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        // Determinar tipo de conexión SSL
        var secureOption = _emailSettings.UseSsl
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;

        await client.ConnectAsync(
            _emailSettings.SmtpHost,
            _emailSettings.SmtpPort,
            secureOption
        );

        // Solo autenticar si hay credenciales configuradas (CoMailpit no las necesita)
        if (!string.IsNullOrWhiteSpace(_emailSettings.SmtpUser) &&
            !string.IsNullOrWhiteSpace(_emailSettings.SmtpPassword))
        {
            await client.AuthenticateAsync(
                _emailSettings.SmtpUser,
                _emailSettings.SmtpPassword
            );
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
