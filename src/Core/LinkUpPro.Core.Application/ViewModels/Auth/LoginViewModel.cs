using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Application.ViewModels.Auth;

public class LoginViewModel
{
    [Required(ErrorMessage = "El campo Usuario es requerido.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "El campo Contraseña es requerido.")]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; }
}
