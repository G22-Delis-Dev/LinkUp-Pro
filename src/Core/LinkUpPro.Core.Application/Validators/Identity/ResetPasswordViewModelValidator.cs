using FluentValidation;
using LinkUpPro.Application.ViewModels.Auth;

namespace LinkUpPro.Application.Validators.Identity;

public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
{
    public ResetPasswordViewModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El correo electrónico no es válido.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("El token de restablecimiento es requerido.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("La contraseña debe tener al menos una letra mayúscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe tener al menos una letra minúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe tener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe tener al menos un carácter especial.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Debe confirmar la contraseña.")
            .Equal(x => x.NewPassword).WithMessage("Las contraseñas no coinciden.");
    }
}
