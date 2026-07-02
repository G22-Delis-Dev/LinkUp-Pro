using FluentValidation;
using LinkUpPro.Application.ViewModels.Comment;

namespace LinkUpPro.Application.Validators.Comment;

public class CreateCommentReplyViewModelValidator : AbstractValidator<CreateCommentReplyViewModel>
{
    public CreateCommentReplyViewModelValidator()
    {
        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage("Selecciona un comentario válido.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("La respuesta no puede estar vacía.")
            .MaximumLength(500).WithMessage("La respuesta no puede exceder los 500 caracteres.");
    }
}
