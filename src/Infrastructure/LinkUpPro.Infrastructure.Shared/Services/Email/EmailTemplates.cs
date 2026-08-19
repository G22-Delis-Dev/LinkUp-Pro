namespace LinkUpPro.Infrastructure.Shared.Services.Email;

public static class EmailTemplates
{
    // Template de activación de cuenta.
    public static string GetActivationTemplate(string userName, string activationUrl)
    {
        return $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Activa tu cuenta</title>
</head>
<body style=""margin:0;padding:0;background-color:#f4f6f9;font-family:'Segoe UI',Tahoma,Geneva,Verdana,sans-serif;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f6f9;padding:40px 20px;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 6px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background:linear-gradient(135deg,#667eea 0%,#764ba2 100%);padding:40px 30px;text-align:center;"">
                            <h1 style=""color:#ffffff;margin:0;font-size:28px;font-weight:700;"">LinkUp Pro</h1>
                            <p style=""color:rgba(255,255,255,0.9);margin:8px 0 0;font-size:14px;"">Tu red social profesional</p>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding:40px 30px;"">
                            <h2 style=""color:#1a1a2e;margin:0 0 16px;font-size:22px;"">¡Bienvenido, {userName}!</h2>
                            <p style=""color:#4a4a68;line-height:1.6;margin:0 0 24px;font-size:15px;"">
                                Gracias por registrarte en <strong>LinkUp Pro</strong>. Para activar tu cuenta y 
                                comenzar a conectar, haz clic en el siguiente botón:
                            </p>
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                    <td align=""center"" style=""padding:8px 0 24px;"">
                                        <a href=""{activationUrl}"" style=""display:inline-block;background:linear-gradient(135deg,#667eea 0%,#764ba2 100%);color:#ffffff;text-decoration:none;padding:14px 40px;border-radius:8px;font-size:16px;font-weight:600;"">
                                            Activar mi cuenta
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            <p style=""color:#8888a0;font-size:13px;line-height:1.5;margin:0 0 16px;"">
                                Si el botón no funciona, copia y pega esta URL en tu navegador:
                            </p>
                            <p style=""color:#667eea;font-size:12px;word-break:break-all;margin:0 0 24px;background:#f8f9ff;padding:12px;border-radius:6px;"">
                                {activationUrl}
                            </p>
                            <p style=""color:#ff6b6b;font-size:13px;margin:0;"">
                                ⏰ Este enlace expira en <strong>24 horas</strong>.
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color:#f8f9ff;padding:20px 30px;text-align:center;border-top:1px solid #e8eaf6;"">
                            <p style=""color:#8888a0;font-size:12px;margin:0;"">
                                Si no creaste esta cuenta, puedes ignorar este correo.
                            </p>
                            <p style=""color:#b0b0c0;font-size:11px;margin:8px 0 0;"">
                                © {DateTime.UtcNow.Year} LinkUp Pro. Todos los derechos reservados.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    // Template de restablecimiento de contraseña.
    public static string GetPasswordResetTemplate(string userName, string resetUrl)
    {
        return $@"
<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Restablecer contraseña</title>
</head>
<body style=""margin:0;padding:0;background-color:#f4f6f9;font-family:'Segoe UI',Tahoma,Geneva,Verdana,sans-serif;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f6f9;padding:40px 20px;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 6px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background:linear-gradient(135deg,#f093fb 0%,#f5576c 100%);padding:40px 30px;text-align:center;"">
                            <h1 style=""color:#ffffff;margin:0;font-size:28px;font-weight:700;"">LinkUp Pro</h1>
                            <p style=""color:rgba(255,255,255,0.9);margin:8px 0 0;font-size:14px;"">Restablecimiento de contraseña</p>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding:40px 30px;"">
                            <h2 style=""color:#1a1a2e;margin:0 0 16px;font-size:22px;"">Hola, {userName}</h2>
                            <p style=""color:#4a4a68;line-height:1.6;margin:0 0 24px;font-size:15px;"">
                                Recibimos una solicitud para restablecer la contraseña de tu cuenta en 
                                <strong>LinkUp Pro</strong>. Haz clic en el siguiente botón para crear una nueva contraseña:
                            </p>
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                    <td align=""center"" style=""padding:8px 0 24px;"">
                                        <a href=""{resetUrl}"" style=""display:inline-block;background:linear-gradient(135deg,#f093fb 0%,#f5576c 100%);color:#ffffff;text-decoration:none;padding:14px 40px;border-radius:8px;font-size:16px;font-weight:600;"">
                                            Restablecer contraseña
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            <p style=""color:#8888a0;font-size:13px;line-height:1.5;margin:0 0 16px;"">
                                Si el botón no funciona, copia y pega esta URL en tu navegador:
                            </p>
                            <p style=""color:#f5576c;font-size:12px;word-break:break-all;margin:0 0 24px;background:#fff5f5;padding:12px;border-radius:6px;"">
                                {resetUrl}
                            </p>
                            <p style=""color:#ff6b6b;font-size:13px;margin:0;"">
                                ⏰ Este enlace expira en <strong>1 hora</strong>.
                            </p>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color:#fff5f7;padding:20px 30px;text-align:center;border-top:1px solid #fce4ec;"">
                            <p style=""color:#8888a0;font-size:12px;margin:0;"">
                                Si no solicitaste este cambio, puedes ignorar este correo. Tu contraseña no cambiará.
                            </p>
                            <p style=""color:#b0b0c0;font-size:11px;margin:8px 0 0;"">
                                © {DateTime.UtcNow.Year} LinkUp Pro. Todos los derechos reservados.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
