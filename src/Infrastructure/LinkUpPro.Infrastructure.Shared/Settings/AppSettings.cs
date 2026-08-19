namespace LinkUpPro.Infrastructure.Shared.Settings;

/// <summary>
/// Configuración general de la aplicación.
/// Se bindea desde appsettings.json sección "AppSettings".
/// </summary>
public class AppSettings
{
    /// <summary>
    /// URL base de la aplicación (para generar enlaces en correos)
    /// </summary>
    public string AppUrl { get; set; } = "https://localhost:7001";

    /// <summary>
    /// Zona horaria para conversión de fechas UTC → local
    /// </summary>
    public string TimeZoneId { get; set; } = "America/Santo_Domingo";

    /// <summary>
    /// Nombre de la aplicación
    /// </summary>
    public string AppName { get; set; } = "LinkUp Pro";
}
