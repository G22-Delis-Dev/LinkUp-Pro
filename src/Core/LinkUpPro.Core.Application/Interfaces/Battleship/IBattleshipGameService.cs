using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Domain.Entities.Battleship;

namespace LinkUpPro.Application.Interfaces.Battleship;

public interface IBattleshipGameService
{
    Task<ServiceResponse<BattleshipGameDto>> CreateGameAsync(Guid creatorId, Guid opponentId);
    Task<BaseResult> SurrenderAsync(Guid gameId, Guid surrenderingUserId);
    Task CheckAndApplyTimeoutAsync(Guid gameId);
    Task<bool> IsParticipantAsync(Guid gameId, Guid userId);
    Task<ServiceResponse<BattleshipGameDto>> GetGameAsync(Guid gameId);
    Task<List<BattleshipGameDto>> GetActiveGamesAsync(Guid userId);
}
