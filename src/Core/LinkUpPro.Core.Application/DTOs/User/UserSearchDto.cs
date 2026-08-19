namespace LinkUpPro.Application.DTOs.User;

public class UserSearchDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Username { get; set; }
    public string? ProfilePictureUrl { get; set; }
}
