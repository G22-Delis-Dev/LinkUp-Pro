using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Application.ViewModels.User;

public class UpdateProfileViewModel
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido.")]
    [MaxLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es requerido.")]
    [RegularExpression(@"^(809|829|849)-\d{3}-\d{4}$",
        ErrorMessage = "El teléfono debe tener formato dominicano (ej: 809-555-1234).")]
    public string PhoneNumber { get; set; } = null!;

    // Foto de perfil opcional — validada en el servicio por magic numbers, tamaño y extensión
    public IFormFile? ProfilePicture { get; set; }

    public string? CurrentProfilePicture { get; set; }

    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
    public string? ConfirmPassword { get; set; }
}
