using LinkUpPro.Application.Interfaces.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipResultController : Controller
{
    private readonly IBattleshipHistoryService _historyService;
    private readonly IBattleshipSetupService _setupService;

    public BattleshipResultController(
        IBattleshipHistoryService historyService,
        IBattleshipSetupService setupService)
    {
        _historyService = historyService;
        _setupService = setupService;
    }

    public async Task<IActionResult> Result(Guid gameId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var game = await _historyService.GetGameDetailsAsync(gameId);
        if (game == null) return NotFound();

        var opponentId = game.Player1Id == currentUserId ? game.Player2Id : game.Player1Id;

        var myBoard = await _setupService.GetBoardAsync(gameId, currentUserId);
        var opponentBoard = await _setupService.GetBoardAsync(gameId, opponentId);

        ViewBag.Game = game;
        ViewBag.OpponentBoard = opponentBoard.Data;
        ViewBag.MyBoard = myBoard.Data;
        ViewBag.CurrentUserId = currentUserId;

        return View("~/Views/Battleship/Result.cshtml");
    }
}
