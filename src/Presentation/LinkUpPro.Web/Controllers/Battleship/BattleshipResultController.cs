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
        var currentUserId = GetCurrentUserId();

        var game = await _historyService.GetGameDetailsAsync(gameId);
        if (game == null) return NotFound();

        var opponentId = game.Player1Id == currentUserId ? game.Player2Id : game.Player1Id;

        var myBoardResult = await _setupService.GetBoardAsync(gameId, currentUserId);
        var opponentBoardResult = await _setupService.GetBoardAsync(gameId, opponentId);

        ViewBag.Game = game;
        ViewBag.OpponentBoard = opponentBoardResult.Data;
        ViewBag.MyBoard = myBoardResult.Data;
        ViewBag.CurrentUserId = currentUserId;

        return View("~/Views/Battleship/Result.cshtml");
    }

    private Guid GetCurrentUserId()
        => Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
}
