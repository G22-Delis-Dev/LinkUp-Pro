using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Application.ViewModels.Auth;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "El formato del correo no es válido (ej. usuario@dominio.com).")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es requerido.")]
    [RegularExpression(@"^(809|829|849)-?\d{3}-?\d{4}$", ErrorMessage = "Debe ser un número de Rep. Dom. válido (ej. 809-555-1234).")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Debes confirmar la contraseña.")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = null!;
}
