using FluentValidation;
using LinkUpPro.Application.ViewModels.Post;

namespace LinkUpPro.Application.Validators.Post;

public class EditPostViewModelValidator : AbstractValidator<EditPostViewModel>
{
    public EditPostViewModelValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("La publicación no es válida.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("El contenido del post es requerido.")
            .MaximumLength(2000).WithMessage("El contenido no puede exceder 2000 caracteres.");
    }
}
