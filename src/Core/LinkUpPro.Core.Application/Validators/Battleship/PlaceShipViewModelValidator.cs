using FluentValidation;
using LinkUpPro.Application.ViewModels.Battleship;
using LinkUpPro.Domain.Enums.Battleship;

namespace LinkUpPro.Application.Validators.Battleship;

public class PlaceShipViewModelValidator : AbstractValidator<PlaceShipViewModel>
{
    public PlaceShipViewModelValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("La partida no es válida.");

        RuleFor(x => x.StartX)
            .InclusiveBetween(0, 11).WithMessage("La posición del barco está fuera del tablero.");

        RuleFor(x => x.StartY)
            .InclusiveBetween(0, 11).WithMessage("La posición del barco está fuera del tablero.");

        RuleFor(x => x.Size)
            .IsInEnum().WithMessage("Tamaño de barco no válido.");

        RuleFor(x => x.Direction)
            .IsInEnum().WithMessage("Dirección no válida.");

        // Regla custom: Verificar que el barco no se sale del tablero al colocarlo
        RuleFor(x => x)
            .Must(ShipFitsOnBoard)
            .WithMessage("El barco se sale de los límites del tablero en esa posición y dirección.");
    }

    private bool ShipFitsOnBoard(PlaceShipViewModel model)
    {
        if (!Enum.IsDefined(typeof(ShipSize), model.Size) || 
            !Enum.IsDefined(typeof(ShipDirection), model.Direction))
        {
            return false; // Ya será capturado por IsInEnum
        }

        int length = (int)model.Size;

        // Validar límites según dirección (tablero 12x12)
        return model.Direction switch
        {
            ShipDirection.Right => (model.StartX + length) <= 12,
            ShipDirection.Left => (model.StartX - length + 1) >= 0,
            ShipDirection.Down => (model.StartY + length) <= 12,
            ShipDirection.Up => (model.StartY - length + 1) >= 0,
            _ => false
        };
    }
}

