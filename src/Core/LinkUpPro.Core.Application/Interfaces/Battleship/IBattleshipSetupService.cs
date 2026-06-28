using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;

namespace LinkUpPro.Application.Interfaces.Battleship;

public interface IBattleshipSetupService
{
    Task<ServiceResponse<ShipDto>> PlaceShipAsync(PlaceShipDto dto);
    Task<ServiceResponse<BattleshipBoardDto>> GetBoardAsync(Guid gameId, Guid playerId);
    Task<BaseResult> ConfirmSetupAsync(Guid gameId, Guid playerId);
}
