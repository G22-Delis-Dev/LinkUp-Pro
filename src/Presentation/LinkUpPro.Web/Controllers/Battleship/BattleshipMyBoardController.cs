using LinkUpPro.Application.Interfaces.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipMyBoardController : Controller
{
    private readonly IBattleshipSetupService _setupService;
    private readonly IBattleshipGameService _gameService;

    public BattleshipMyBoardController(
        IBattleshipSetupService setupService,
        IBattleshipGameService gameService)
    {
        _setupService = setupService;
        _gameService = gameService;
    }

    // Retorna partial view con el tablero propio actualizado
    public async Task<IActionResult> Board(Guid gameId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var board = await _setupService.GetBoardAsync(gameId, currentUserId);
        var game = await _gameService.GetGameAsync(gameId);

        if (board.HasError || game.HasError)
            return NotFound();

        ViewBag.Game = game.Data;
        return PartialView("~/Views/Battleship/_MyBoard.cshtml", board.Data);
    }
}
