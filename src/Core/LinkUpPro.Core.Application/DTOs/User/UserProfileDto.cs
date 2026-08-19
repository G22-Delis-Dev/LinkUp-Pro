namespace LinkUpPro.Application.DTOs.User;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; }
}
