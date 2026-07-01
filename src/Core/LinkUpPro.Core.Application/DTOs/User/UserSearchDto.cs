namespace LinkUpPro.Application.DTOs.User;

public class UserSearchDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
}
