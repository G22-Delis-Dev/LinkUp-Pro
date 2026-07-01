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
        var currentUserId = GetCurrentUserId();

        var boardResult = await _setupService.GetBoardAsync(gameId, currentUserId);
        var gameResult = await _gameService.GetGameAsync(gameId);

        if (boardResult.HasError || gameResult.HasError)
            return NotFound();

        ViewBag.Game = gameResult.Data;
        return PartialView("~/Views/Battleship/_MyBoard.cshtml", boardResult.Data);
    }

    private Guid GetCurrentUserId()
        => Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
}
