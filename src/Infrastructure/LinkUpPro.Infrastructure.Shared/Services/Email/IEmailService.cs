namespace LinkUpPro.Infrastructure.Shared.Services.Email;

// Interface del servicio de envío de correos electrónicos.
public interface IEmailService
{
    // Envía el correo de activación de cuenta con token.
    Task SendActivationEmailAsync(string toEmail, string userName, string activationUrl);

    // Envía el correo de restablecimiento de contraseña con token.
    Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetUrl);

    // Envía un correo genérico (para otros usos futuros).
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
}
