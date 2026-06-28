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

public class RegisterService : IRegisterService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public RegisterService(
        UserManager<AppUser> userManager,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<ServiceResponse<string>> RegisterAsync(RegisterDto dto)
    {
        // 1. Validar unicidad case-insensitive de username
        var existingByUsername = await _userManager.FindByNameAsync(dto.Username);
        if (existingByUsername != null)
        {
            return ServiceResponse<string>.Failure("El nombre de usuario ya está en uso.");
        }

        // 2. Validar unicidad case-insensitive de email
        var existingByEmail = await _userManager.FindByEmailAsync(dto.Email);
        if (existingByEmail != null)
        {
            return ServiceResponse<string>.Failure("El correo electrónico ya está registrado.");
        }

        // 3. Crear la entidad User del dominio primero
        var domainUser = new Domain.Entities.User.User
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            IsActive = false, // Se activa con el correo de activación
            AppUserId = string.Empty // Se actualiza después de crear AppUser
        };
        await _userRepository.AddAsync(domainUser);

        // 4. Crear AppUser en Identity
        var appUser = new AppUser
        {
            UserName = dto.Username.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            IsActive = false,
            UserId = domainUser.Id
        };

        var createResult = await _userManager.CreateAsync(appUser, dto.Password);
        if (!createResult.Succeeded)
        {
            // Rollback: eliminar el domain user si falla la creación de AppUser
            await _userRepository.DeleteAsync(domainUser);
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            return ServiceResponse<string>.Failure($"Error al crear la cuenta: {errors}");
        }

        // 5. Asignar rol de usuario
        await _userManager.AddToRoleAsync(appUser, "User");

        // 6. Actualizar AppUserId en dominio
        domainUser.AppUserId = appUser.Id.ToString();
        await _userRepository.UpdateAsync(domainUser);

        // 7. Generar token de activación y enviar correo
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        // La URL de activación se construirá en el controlador con Url.Action
        // Aquí solo enviamos el correo con el token codificado
        var activationUrl = $"/Auth/ActivateAccount?userId={appUser.Id}&token={encodedToken}";

        try
        {
            await _emailService.SendActivationEmailAsync(
                appUser.Email!,
                $"{dto.FirstName} {dto.LastName}",
                activationUrl);
        }
        catch
        {
            // No falla el registro si el correo no se envía
            // El usuario podrá reenviar desde la pantalla de login
        }

        return ServiceResponse<string>.Success(
            "Registro exitoso. Revise su correo electrónico para activar su cuenta.");
    }
}
