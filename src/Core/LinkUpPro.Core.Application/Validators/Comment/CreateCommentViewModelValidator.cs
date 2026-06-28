using FluentValidation;
using LinkUpPro.Application.ViewModels.Comment;

namespace LinkUpPro.Application.Validators.Comment;

public class CreateCommentViewModelValidator : AbstractValidator<CreateCommentViewModel>
{
    public CreateCommentViewModelValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("El ID del post es requerido.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("El comentario no puede estar vacío.")
            .MaximumLength(500).WithMessage("El comentario no puede exceder los 500 caracteres.");
    }
}
