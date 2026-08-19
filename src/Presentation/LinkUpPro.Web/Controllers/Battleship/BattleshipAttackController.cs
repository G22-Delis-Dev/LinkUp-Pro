using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Domain.Enums.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipAttackController : Controller
{
    private readonly IBattleshipAttackService _attackService;
    private readonly IBattleshipGameService _gameService;

    public BattleshipAttackController(
        IBattleshipAttackService attackService,
        IBattleshipGameService gameService)
    {
        _attackService = attackService;
        _gameService = gameService;
    }

    // GET: ver tablero de ataque / refrescar estado
    [HttpGet]
    public async Task<IActionResult> Index(Guid gameId)
    {
        var userId = GetCurrentUserId();

        var isParticipant = await _gameService.IsParticipantAsync(gameId, userId);
        if (!isParticipant) return Forbid();

        await _gameService.CheckAndApplyTimeoutAsync(gameId);

        var gameResult = await _gameService.GetGameAsync(gameId);
        if (gameResult.HasError || gameResult.Data == null)
            return RedirectToAction("Index", "BattleshipGameList");

        var game = gameResult.Data;

        if (game.Status == GameStatus.Finished)
            return RedirectToAction("Result", "BattleshipResult", new { gameId });

        if (game.Status == GameStatus.PlacingShips)
            return RedirectToAction("Setup", "BattleshipSetup", new { gameId });

        var boardResult = await _attackService.GetOpponentBoardAsync(gameId, userId);
        if (boardResult.HasError || boardResult.Data == null)
        {
            TempData["Error"] = "No se pudo cargar el tablero del oponente.";
            return RedirectToAction("Index", "BattleshipGameList");
        }

        ViewBag.Game = game;
        ViewBag.OpponentBoard = boardResult.Data;
        ViewBag.CurrentUserId = userId;

        return View("~/Views/Battleship/Attack.cshtml");
    }

    // POST: registrar ataque (AJAX)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Attack([FromBody] LinkUpPro.Application.ViewModels.Battleship.AttackViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Datos inválidos" });

        var userId = GetCurrentUserId();
        var result = await _attackService.AttackAsync(model.GameId, userId, model.TargetY, model.TargetX);

        if (result.HasError)
            return Json(new { success = false, message = result.Error });

        return Json(new { success = true, result = result.Data });
    }

    // POST: refrescar pantalla -> redirige al GET
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Refresh(Guid gameId)
        => RedirectToAction(nameof(Index), new { gameId });

    private Guid GetCurrentUserId()
        => Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
}
