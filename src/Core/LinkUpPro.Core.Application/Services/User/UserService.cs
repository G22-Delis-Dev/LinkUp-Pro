using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.User;
using LinkUpPro.Application.Interfaces.User;
using LinkUpPro.Domain.Interfaces.Repositories.User;
using LinkUpPro.Infrastructure.Identity.Entities;
using LinkUpPro.Infrastructure.Shared.Services.Storage;
using Microsoft.AspNetCore.Identity;

namespace LinkUpPro.Application.Services.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IImageStorageService _imageStorage;

    public UserService(
        IUserRepository userRepository,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IImageStorageService imageStorage)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _signInManager = signInManager;
        _imageStorage = imageStorage;
    }

    public async Task<ServiceResponse<UserProfileDto>> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ServiceResponse<UserProfileDto>.Failure("Usuario no encontrado.");
        }

        var appUser = await _userManager.FindByIdAsync(user.AppUserId);
        if (appUser == null)
        {
            return ServiceResponse<UserProfileDto>.Failure("Cuenta de usuario no encontrada.");
        }

        var dto = new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = appUser.UserName!,
            Email = appUser.Email!,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = !string.IsNullOrEmpty(user.ProfilePicturePath)
                ? _imageStorage.GetImageUrl(user.ProfilePicturePath)
                : null,
            IsActive = user.IsActive
        };

        return ServiceResponse<UserProfileDto>.Success(dto);
    }

    public async Task<BaseResult> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return BaseResult.Fail("Usuario no encontrado.");
        }

        // Solo actualizar campos permitidos (Username y Email son READONLY, blindado en backend)
        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();
        user.PhoneNumber = dto.PhoneNumber.Trim();

        await _userRepository.UpdateAsync(user);

        // Sincronizar nombres en AppUser
        var appUser = await _userManager.FindByIdAsync(user.AppUserId);
        if (appUser != null)
        {
            appUser.FirstName = dto.FirstName.Trim();
            appUser.LastName = dto.LastName.Trim();
            await _userManager.UpdateAsync(appUser);
        }

        return BaseResult.Ok();
    }

    public async Task<BaseResult> ChangeProfilePictureAsync(
        Guid userId, Stream imageStream, string contentType, string? fileName)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return BaseResult.Fail("Usuario no encontrado.");
        }

        // Guardar la imagen anterior para posible rollback
        var previousPicture = user.ProfilePicturePath;

        try
        {
            // Guardar nueva imagen (valida internamente extensión, tamaño, magic numbers)
            var newPath = await _imageStorage.SaveImageAsync(imageStream, contentType, fileName);

            // Actualizar perfil
            user.ProfilePicturePath = newPath;
            await _userRepository.UpdateAsync(user);

            // Borrar imagen anterior
            if (!string.IsNullOrEmpty(previousPicture))
            {
                await _imageStorage.DeleteImageAsync(previousPicture);
            }

            return BaseResult.Ok();
        }
        catch (InvalidImageException ex)
        {
            return BaseResult.Fail(ex.Message);
        }
        catch
        {
            // Rollback: restaurar imagen anterior
            user.ProfilePicturePath = previousPicture;
            await _userRepository.UpdateAsync(user);
            return BaseResult.Fail("Ocurrió un error al procesar la imagen. Inténtelo nuevamente.");
        }
    }

    public async Task<BaseResult> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        // Si campos vacíos, conserva contraseña actual
        if (string.IsNullOrWhiteSpace(dto.CurrentPassword) &&
            string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            return BaseResult.Ok();
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return BaseResult.Fail("Usuario no encontrado.");
        }

        var appUser = await _userManager.FindByIdAsync(user.AppUserId);
        if (appUser == null)
        {
            return BaseResult.Fail("Cuenta de usuario no encontrada.");
        }

        var result = await _userManager.ChangePasswordAsync(
            appUser, dto.CurrentPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BaseResult.Fail(errors);
        }

        // Invalidar sesiones previas y re-login
        await _userManager.UpdateSecurityStampAsync(appUser);
        await _signInManager.RefreshSignInAsync(appUser);

        return BaseResult.Ok();
    }
}
