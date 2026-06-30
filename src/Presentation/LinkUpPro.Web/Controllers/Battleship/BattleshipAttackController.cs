using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Application.ViewModels.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipAttackController : Controller
{
    private readonly IBattleshipGameService _gameService;
    private readonly IBattleshipAttackService _attackService;
    private readonly IBattleshipSetupService _setupService;

    public BattleshipAttackController(
        IBattleshipGameService gameService,
        IBattleshipAttackService attackService,
        IBattleshipSetupService setupService)
    {
        _gameService = gameService;
        _attackService = attackService;
        _setupService = setupService;
    }

    public async Task<IActionResult> Board(Guid gameId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var game = await _gameService.GetGameAsync(gameId);
        if (game.HasError || game.Data == null)
            return RedirectToAction("Index", "BattleshipGameList");

        if (game.Data.Status == LinkUpPro.Domain.Enums.Battleship.GameStatus.Finished)
            return RedirectToAction("Result", "BattleshipResult", new { gameId });

        var opponentId = game.Data.Player1Id == currentUserId ? game.Data.Player2Id : game.Data.Player1Id;
        
        // Obtenemos los ataques que hemos realizado (ataques recibidos por el oponente)
        var opponentBoard = await _setupService.GetBoardAsync(gameId, opponentId);
        
        ViewBag.Game = game.Data;
        ViewBag.OpponentBoard = opponentBoard.Data;
        ViewBag.CurrentUserId = currentUserId;

        return View("~/Views/Battleship/Attack.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Attack([FromBody] AttackViewModel model)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var dto = new AttackDto
        {
            GameId = model.GameId,
            AttackerPlayerId = currentUserId,
            TargetX = model.TargetX,
            TargetY = model.TargetY
        };

        var result = await _attackService.AttackAsync(dto);

        if (result.HasError)
        {
            return Json(new { success = false, message = result.Error });
        }

        return Json(new { success = true, result = result.Data });
    }
}
