using LinkUpPro.Application.Interfaces.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipSetupController : Controller
{
    private readonly IBattleshipGameService _gameService;
    private readonly IBattleshipSetupService _setupService;

    public BattleshipSetupController(
        IBattleshipGameService gameService,
        IBattleshipSetupService setupService)
    {
        _gameService = gameService;
        _setupService = setupService;
    }

    public async Task<IActionResult> Setup(Guid gameId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var game = await _gameService.GetGameAsync(gameId);
        if (game.HasError || game.Data == null)
            return RedirectToAction("Index", "BattleshipGameList");

        if (game.Data.Status != LinkUpPro.Domain.Enums.Battleship.GameStatus.PlacingShips)
        {
            if (game.Data.Status == LinkUpPro.Domain.Enums.Battleship.GameStatus.InProgress)
                return RedirectToAction("Board", "BattleshipAttack", new { gameId });
                
            return RedirectToAction("Index", "BattleshipGameList");
        }

        var board = await _setupService.GetBoardAsync(gameId, currentUserId);
        
        // Si ya colocó sus 5 barcos pero el oponente no, mostrar pantalla de espera
        if (!board.HasError && board.Data != null && board.Data.Ships.Count >= 5)
        {
            ViewBag.GameId = gameId;
            return View("~/Views/Battleship/Waiting.cshtml");
        }

        ViewBag.GameId = gameId;
        ViewBag.Board = board.Data;
        return View("~/Views/Battleship/Setup.cshtml");
    }
}
