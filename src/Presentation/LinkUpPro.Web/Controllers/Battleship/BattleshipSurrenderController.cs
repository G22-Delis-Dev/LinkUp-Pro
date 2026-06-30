using LinkUpPro.Application.Interfaces.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipSurrenderController : Controller
{
    private readonly IBattleshipGameService _gameService;

    public BattleshipSurrenderController(IBattleshipGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Surrender(Guid gameId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var result = await _gameService.CancelGameAsync(gameId, currentUserId);

        if (result.Success)
            TempData["Success"] = "Te has rendido de la partida.";
        else
            TempData["Error"] = result.Errors.FirstOrDefault() ?? "Error al rendirse.";

        return RedirectToAction("Index", "BattleshipGameList");
    }
}
