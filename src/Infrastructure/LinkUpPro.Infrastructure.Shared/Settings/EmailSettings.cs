namespace LinkUpPro.Infrastructure.Shared.Settings;

/// <summary>
/// Configuración SMTP para envío de correos.
/// Se bindea desde appsettings.json sección "EmailSettings".
/// </summary>
public class EmailSettings
{
    public string SmtpHost { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 1025;
    public string? SmtpUser { get; set; }
    public string? SmtpPassword { get; set; }
    public string FromEmail { get; set; } = "noreply@linkuppro.com";
    public string FromName { get; set; } = "LinkUp Pro";
    public bool UseSsl { get; set; } = false;
}
