using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.User;

namespace LinkUpPro.Application.Interfaces.User;

public interface IUserService
{
    Task<ServiceResponse<UserProfileDto>> GetProfileAsync(Guid userId);
    Task<BaseResult> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
    Task<BaseResult> ChangeProfilePictureAsync(Guid userId, Stream imageStream, string contentType, string? fileName);
    Task<BaseResult> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);

    /// <summary>
    /// Busca usuarios activos por nombre o apellido.
    /// Si excludeFriendsAndPending es true, excluye a quienes ya son amigos o tienen solicitud pendiente.
    /// </summary>
    Task<List<UserSearchDto>> SearchUsersAsync(string query, Guid currentUserId, bool excludeFriendsAndPending = false);
}
