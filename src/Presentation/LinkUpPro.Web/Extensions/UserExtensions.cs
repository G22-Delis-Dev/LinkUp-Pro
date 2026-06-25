using System.Security.Claims;

namespace LinkUpPro.Web.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to simplify user data access.
/// </summary>
public static class UserExtensions
{
    /// <summary>
    /// Gets the current user's ID from claims.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal representing the current user.</param>
    /// <returns>The user ID as a Guid.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user ID claim is not found or invalid.</exception>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new InvalidOperationException("User ID claim not found.");
        }

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException("User ID claim is not a valid GUID.");
        }

        return userId;
    }

    /// <summary>
    /// Tries to get the current user's ID from claims.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal representing the current user.</param>
    /// <param name="userId">The user ID if found and valid; otherwise, Guid.Empty.</param>
    /// <returns>True if the user ID was successfully retrieved; otherwise, false.</returns>
    public static bool TryGetUserId(this ClaimsPrincipal user, out Guid userId)
    {
        userId = Guid.Empty;
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return false;
        }

        return Guid.TryParse(userIdClaim, out userId);
    }

    /// <summary>
    /// Gets the current user's username from claims.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal representing the current user.</param>
    /// <returns>The username if found; otherwise, null.</returns>
    public static string? GetUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// Gets the current user's email from claims.
    /// </summary>
    /// <param name="user">The ClaimsPrincipal representing the current user.</param>
    /// <returns>The email if found; otherwise, null.</returns>
    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }
}
