namespace LinkUpPro.Application.Interfaces.Battleship;

public interface IBattleshipHubService
{
    Task NotifyGameUpdatedAsync(Guid gameId);
    Task NotifyTurnChangedAsync(Guid gameId, Guid currentPlayerId);
    Task NotifyGameOverAsync(Guid gameId, Guid? winnerId);
}
