using FluentValidation;
using LinkUpPro.Application.ViewModels.Auth;

namespace LinkUpPro.Application.Validators.Identity;

public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
{
    public ForgotPasswordViewModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .EmailAddress().WithMessage("El correo electrónico no es válido.");
    }
}
