using LinkUpPro.Application.Interfaces.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipHistoryController : Controller
{
    private readonly IBattleshipHistoryService _historyService;

    public BattleshipHistoryController(IBattleshipHistoryService historyService)
    {
        _historyService = historyService;
    }

    public async Task<IActionResult> Index()
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var history = await _historyService.GetGameHistoryAsync(currentUserId);

        var finishedGames = history.Where(g => 
            g.Status == LinkUpPro.Domain.Enums.Battleship.GameStatus.Finished || 
            g.Status == LinkUpPro.Domain.Enums.Battleship.GameStatus.Canceled).ToList();

        ViewBag.TotalPlayed = finishedGames.Count;
        ViewBag.TotalWon = finishedGames.Count(g => g.WinnerId == currentUserId);
        ViewBag.TotalLost = finishedGames.Count - ViewBag.TotalWon;

        return View("~/Views/Battleship/History.cshtml", finishedGames);
    }
}
