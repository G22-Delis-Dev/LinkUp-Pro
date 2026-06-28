using FluentValidation;
using LinkUpPro.Application.ViewModels.Battleship;
using LinkUpPro.Domain.Enums.Battleship;

namespace LinkUpPro.Application.Validators.Battleship;

public class PlaceShipViewModelValidator : AbstractValidator<PlaceShipViewModel>
{
    public PlaceShipViewModelValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty().WithMessage("El ID del juego es requerido.");

        RuleFor(x => x.StartX)
            .InclusiveBetween(0, 9).WithMessage("La coordenada X debe estar entre 0 y 9.");

        RuleFor(x => x.StartY)
            .InclusiveBetween(0, 9).WithMessage("La coordenada Y debe estar entre 0 y 9.");

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

        if (model.Direction == ShipDirection.Horizontal)
        {
            return (model.StartX + length) <= 10;
        }
        else // Vertical
        {
            return (model.StartY + length) <= 10;
        }
    }
}

