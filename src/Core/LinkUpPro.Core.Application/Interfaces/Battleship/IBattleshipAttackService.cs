using LinkUpPro.Application.Common;
using LinkUpPro.Application.DTOs.Battleship;

namespace LinkUpPro.Application.Interfaces.Battleship;

public interface IBattleshipAttackService
{
    Task<ServiceResponse<AttackResultDto>> AttackAsync(Guid gameId, Guid attackerId, int row, int col);
    Task<ServiceResponse<BattleshipBoardDto>> GetOpponentBoardAsync(Guid gameId, Guid currentUserId);
}
