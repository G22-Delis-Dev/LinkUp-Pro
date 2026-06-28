using FluentValidation;
using LinkUpPro.Application.ViewModels.Auth;

namespace LinkUpPro.Application.Validators.Identity;

public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterViewModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100).WithMessage("El apellido no puede exceder 100 caracteres.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es requerido.")
            .MaximumLength(50).WithMessage("El usuario no puede exceder 50 caracteres.")
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("El usuario solo puede contener letras, números y guion bajo.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El correo electrónico no es válido.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("El número de teléfono es requerido.")
            .Matches(@"^(809|829|849)-\d{3}-\d{4}$")
            .WithMessage("El teléfono debe tener formato dominicano (ej: 809-555-1234).");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("La contraseña debe tener al menos una letra mayúscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe tener al menos una letra minúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe tener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe tener al menos un carácter especial.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Debe confirmar la contraseña.")
            .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden.");
    }
}
