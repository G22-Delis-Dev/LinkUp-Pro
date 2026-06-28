using FluentValidation;
using LinkUpPro.Application.ViewModels.Battleship;

namespace LinkUpPro.Application.Validators.Battleship;

public class CreateGameViewModelValidator : AbstractValidator<CreateGameViewModel>
{
    public CreateGameViewModelValidator()
    {
        RuleFor(x => x.OpponentId)
            .NotEmpty().WithMessage("Debe seleccionar un oponente válido.");
    }
}
