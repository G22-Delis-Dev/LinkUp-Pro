using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Auth;
using LinkUpPro.Application.Interfaces.Identity;
using LinkUpPro.Infrastructure.Identity.Entities;
using LinkUpPro.Infrastructure.Shared.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace LinkUpPro.Application.Services.Identity;

public class PasswordResetService : IPasswordResetService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailService _emailService;

    public PasswordResetService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    public async Task<ServiceResponse<string>> RequestResetAsync(ForgotPasswordDto dto)
    {
        // Mensaje genérico para no revelar si la cuenta existe (requisito de seguridad)
        const string genericMessage =
            "Si el correo está registrado, recibirá un enlace para restablecer su contraseña.";

        var appUser = await _userManager.FindByEmailAsync(dto.Email);
        if (appUser == null)
        {
            return ServiceResponse<string>.Success(genericMessage);
        }

        // Generar token de reset (1h de vigencia se configura por separado si es necesario)
        var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var resetUrl = $"{dto.Origin}/Auth/ResetPassword?email={appUser.Email}&token={encodedToken}";

        try
        {
            await _emailService.SendPasswordResetEmailAsync(
                appUser.Email!,
                $"{appUser.FirstName} {appUser.LastName}",
                resetUrl);
        }
        catch
        {
            // Silenciar errores de envío
        }

        return ServiceResponse<string>.Success(genericMessage);
    }

    public async Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var appUser = await _userManager.FindByEmailAsync(dto.Email);
        if (appUser == null)
        {
            return ServiceResponse<string>.Failure(
                "El enlace de restablecimiento no es válido.");
        }

        // Decodificar token
        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));

        // Resetear password
        var result = await _userManager.ResetPasswordAsync(appUser, decodedToken, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResponse<string>.Failure(
                $"No se pudo restablecer la contraseña: {errors}");
        }

        // Invalidar sesiones anteriores cambiando el SecurityStamp
        await _userManager.UpdateSecurityStampAsync(appUser);

        return ServiceResponse<string>.Success(
            "Su contraseña ha sido restablecida exitosamente. Ya puede iniciar sesión con su nueva contraseña.");
    }
}
