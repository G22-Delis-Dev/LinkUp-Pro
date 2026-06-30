using LinkUpPro.Application.Interfaces.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipGameListController : Controller
{
    private readonly IBattleshipGameService _gameService;
    private readonly IBattleshipHistoryService _historyService;

    public BattleshipGameListController(
        IBattleshipGameService gameService,
        IBattleshipHistoryService historyService)
    {
        _gameService = gameService;
        _historyService = historyService;
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var activeGames = await _gameService.GetActiveGamesAsync(currentUserId);
        var history = await _historyService.GetGameHistoryAsync(currentUserId);

        ViewBag.ActiveGames = activeGames;
        ViewBag.History = history;

        return View("~/Views/Battleship/Index.cshtml");
    }
}
