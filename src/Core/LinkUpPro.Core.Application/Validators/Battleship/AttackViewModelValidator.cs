using FluentValidation;
using LinkUpPro.Application.ViewModels.Battleship;

namespace LinkUpPro.Application.Validators.Battleship;

public class AttackViewModelValidator : AbstractValidator<AttackViewModel>
{
    public AttackViewModelValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("La partida no es válida.");

        RuleFor(x => x.TargetX)
            .InclusiveBetween(0, 11).WithMessage("Coordenada de ataque fuera del tablero.");

        RuleFor(x => x.TargetY)
            .InclusiveBetween(0, 11).WithMessage("Coordenada de ataque fuera del tablero.");
    }
}
