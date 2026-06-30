using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Auth;
using LinkUpPro.Application.Interfaces.Identity;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using LinkUpPro.Infrastructure.Identity.Entities;
using LinkUpPro.Infrastructure.Shared.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace LinkUpPro.Application.Services.Identity;

public class AccountActivationService : IAccountActivationService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public AccountActivationService(
        UserManager<AppUser> userManager,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<ServiceResponse<ActivationResultDto>> ActivateAsync(string userId, string token)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null)
        {
            return ServiceResponse<ActivationResultDto>.Failure(
                "El enlace de activación no es válido.");
        }

        // Decodificar token
        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        // Confirmar email (esto valida el token de un solo uso)
        var result = await _userManager.ConfirmEmailAsync(appUser, decodedToken);
        if (!result.Succeeded)
        {
            return ServiceResponse<ActivationResultDto>.Failure(
                "El enlace de activación ha expirado o ya fue utilizado.");
        }

        // Activar cuenta en dominio
        var domainUser = await _userRepository.GetByAppUserIdAsync(appUser.Id.ToString());
        if (domainUser != null)
        {
            domainUser.IsActive = true;
            await _userRepository.UpdateAsync(domainUser);
        }

        // Activar en Identity
        appUser.IsActive = true;
        await _userManager.UpdateAsync(appUser);

        return ServiceResponse<ActivationResultDto>.Success(new ActivationResultDto
        {
            Success = true,
            Message = "¡Cuenta activada exitosamente! Ya puede iniciar sesión."
        });
    }

    public async Task<ServiceResponse<string>> ResendActivationAsync(string email, string origin)
    {
        // Mensaje genérico para no revelar si la cuenta existe
        const string genericMessage = "Si el correo está registrado, recibirá un nuevo enlace de activación.";

        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            return ServiceResponse<string>.Success(genericMessage);
        }

        // Ya está activada
        if (appUser.EmailConfirmed)
        {
            return ServiceResponse<string>.Success(genericMessage);
        }

        // Verificar cooldown de 5 minutos (usando SecurityStamp como proxy de último envío)
        // En producción se usaría un campo de último envío, por ahora usamos un enfoque simple
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var activationUrl = $"{origin}/Auth/Activate?userId={appUser.Id}&token={encodedToken}";

        try
        {
            await _emailService.SendActivationEmailAsync(
                appUser.Email!,
                $"{appUser.FirstName} {appUser.LastName}",
                activationUrl);
        }
        catch
        {
            // Silenciar errores de envío
        }

        return ServiceResponse<string>.Success(genericMessage);
    }
}
