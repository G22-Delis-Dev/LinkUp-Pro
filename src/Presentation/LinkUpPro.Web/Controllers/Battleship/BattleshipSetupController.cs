using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Domain.Enums.Battleship;
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

    [HttpGet]
    public async Task<IActionResult> Setup(Guid gameId)
    {
        var currentUserId = GetCurrentUserId();

        var gameResult = await _gameService.GetGameAsync(gameId);
        if (gameResult.HasError || gameResult.Data == null)
            return RedirectToAction("Index", "BattleshipGameList");

        var game = gameResult.Data;

        if (game.Status == GameStatus.Finished || game.Status == GameStatus.Canceled)
            return RedirectToAction("Index", "BattleshipGameList");

        if (game.Status == GameStatus.InProgress)
            return RedirectToAction("Index", "BattleshipAttack", new { gameId });

        var boardResult = await _setupService.GetBoardAsync(gameId, currentUserId);

        // Si ya colocó sus 5 barcos (posición >= 0), mostrar pantalla de espera
        var placedCount = boardResult.Data?.Ships.Count(s => s.StartX >= 0 && s.StartY >= 0) ?? 0;
        if (!boardResult.HasError && boardResult.Data != null && placedCount >= 5)
        {
            ViewBag.GameId = gameId;
            return View("~/Views/Battleship/Waiting.cshtml");
        }

        ViewBag.GameId = gameId;
        ViewBag.Board = boardResult.Data;
        return View("~/Views/Battleship/Setup.cshtml");
    }

    // POST: refrescar estado de espera -> verificar si ambos listos
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Refresh(Guid gameId)
    {
        var bothReady = await _setupService.BothPlayersReadyAsync(gameId);

        if (bothReady)
            return RedirectToAction("Index", "BattleshipAttack", new { gameId });

        return RedirectToAction(nameof(Setup), new { gameId });
    }

    private Guid GetCurrentUserId()
        => Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
}
