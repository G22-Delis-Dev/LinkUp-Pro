using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;

namespace LinkUpPro.Application.Interfaces.Battleship;

public interface IBattleshipGameService
{
    Task<ServiceResponse<BattleshipGameDto>> CreateGameAsync(Guid player1Id, Guid opponentId);
    Task<ServiceResponse<BattleshipGameDto>> GetGameAsync(Guid gameId);
    Task<List<BattleshipGameDto>> GetActiveGamesAsync(Guid playerId);
    Task<BaseResult> CancelGameAsync(Guid gameId, Guid playerId);
}
