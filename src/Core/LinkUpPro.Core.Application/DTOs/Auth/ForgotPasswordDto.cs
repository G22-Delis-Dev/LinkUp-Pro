namespace LinkUpPro.Application.DTOs.Auth;

public class ForgotPasswordDto
{
    public string Email { get; set; } = null!;
    public string Origin { get; set; } = string.Empty;
}
