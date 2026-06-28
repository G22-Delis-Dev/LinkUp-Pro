using LinkUpPro.Application.DTOs.Battleship;

namespace LinkUpPro.Application.Interfaces.Battleship;

public interface IBattleshipHistoryService
{
    Task<List<BattleshipGameDto>> GetGameHistoryAsync(Guid playerId);
    Task<BattleshipGameDto?> GetGameDetailsAsync(Guid gameId);
}
