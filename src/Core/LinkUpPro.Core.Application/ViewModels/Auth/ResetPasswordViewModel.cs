using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Application.ViewModels.Auth;

public class ResetPasswordViewModel
{
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    public string Token { get; set; } = null!;
    
    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$", ErrorMessage = "La contraseña debe tener mayúsculas, minúsculas, números y caracteres especiales.")]
    public string NewPassword { get; set; } = null!;
    
    [Required(ErrorMessage = "Debes confirmar la contraseña.")]
    [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = null!;
}
