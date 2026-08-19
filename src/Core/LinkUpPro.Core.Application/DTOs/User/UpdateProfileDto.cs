namespace LinkUpPro.Application.DTOs.User;

public class UpdateProfileDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    // Username y Email son solo lectura, blindados en backend
}
