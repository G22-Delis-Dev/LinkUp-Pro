using FluentValidation;
using LinkUpPro.Application.ViewModels.Post;

namespace LinkUpPro.Application.Validators.Post;

public class CreatePostViewModelValidator : AbstractValidator<CreatePostViewModel>
{
    public CreatePostViewModelValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("El contenido del post es requerido.")
            .MaximumLength(2000).WithMessage("El contenido no puede exceder 2000 caracteres.");

        // Debe tener imagen O video, pero no ambos
        RuleFor(x => x)
            .Must(x => x.Image != null || !string.IsNullOrWhiteSpace(x.YouTubeUrl))
            .WithMessage("Debe incluir una imagen o un enlace de video de YouTube.")
            .Must(x => !(x.Image != null && !string.IsNullOrWhiteSpace(x.YouTubeUrl)))
            .WithMessage("No puede incluir imagen y video al mismo tiempo.");

        // Validar tamaño de imagen si se proporciona
        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image!.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024)
                .WithMessage("La imagen no puede exceder 5 MB.");

            RuleFor(x => x.Image!.ContentType)
                .Must(ct => new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" }.Contains(ct.ToLowerInvariant()))
                .WithMessage("Solo se permiten imágenes JPG, PNG o WebP.");
        });

        // Validar URL de YouTube si se proporciona
        When(x => !string.IsNullOrWhiteSpace(x.YouTubeUrl), () =>
        {
            RuleFor(x => x.YouTubeUrl!)
                .Matches(@"(youtube\.com|youtu\.be)")
                .WithMessage("Debe ser una URL válida de YouTube.");
        });
    }
}
