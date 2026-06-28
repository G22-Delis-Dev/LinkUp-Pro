using FluentValidation;
using LinkUpPro.Application.ViewModels.Battleship;

namespace LinkUpPro.Application.Validators.Battleship;

public class AttackViewModelValidator : AbstractValidator<AttackViewModel>
{
    public AttackViewModelValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("El ID del juego es requerido.");

        RuleFor(x => x.TargetX)
            .InclusiveBetween(0, 9).WithMessage("La coordenada X debe estar entre 0 y 9.");

        RuleFor(x => x.TargetY)
            .InclusiveBetween(0, 9).WithMessage("La coordenada Y debe estar entre 0 y 9.");
    }
}
