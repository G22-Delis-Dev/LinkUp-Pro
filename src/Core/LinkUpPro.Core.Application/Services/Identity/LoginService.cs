using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Auth;
using LinkUpPro.Application.Interfaces.Identity;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using LinkUpPro.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace LinkUpPro.Application.Services.Identity;

public class LoginService : ILoginService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IUserRepository _userRepository;

    public LoginService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IUserRepository userRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
    }

    public async Task<ServiceResponse<string>> LoginAsync(LoginDto dto)
    {
        // 1. Buscar usuario por username (case-insensitive via Identity)
        var appUser = await _userManager.FindByNameAsync(dto.Username);
        
        if (appUser == null)
        {
            return ServiceResponse<string>.Failure(
                "El nombre de usuario o la contraseña son incorrectos.");
        }

        // 2. Verificar cuenta activa en dominio
        var domainUser = await _userRepository.GetByAppUserIdAsync(appUser.Id.ToString());
        if (domainUser != null && !domainUser.IsActive)
        {
            return ServiceResponse<string>.Failure(
                "Su cuenta se encuentra inactiva. Verifique su correo de activación.");
        }

        // 3. Verificar lockout
        if (await _userManager.IsLockedOutAsync(appUser))
        {
            return ServiceResponse<string>.Failure(
                "Su cuenta ha sido bloqueada temporalmente por múltiples intentos fallidos. Intente nuevamente en 15 minutos.");
        }

        // 4. Intentar login con SignInManager
        var result = await _signInManager.PasswordSignInAsync(
            appUser,
            dto.Password,
            isPersistent: dto.RememberMe,   // Cookie de 7 días si RememberMe
            lockoutOnFailure: true);          // Incrementa intentos fallidos

        if (result.IsLockedOut)
        {
            return ServiceResponse<string>.Failure(
                "Su cuenta ha sido bloqueada temporalmente por múltiples intentos fallidos. Intente nuevamente en 15 minutos.");
        }

        if (!result.Succeeded)
        {
            return ServiceResponse<string>.Failure(
                "El nombre de usuario o la contraseña son incorrectos.");
        }

        return ServiceResponse<string>.Success("Inicio de sesión exitoso.");
    }
}
