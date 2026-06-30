using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Application.ViewModels.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipGameCreateController : Controller
{
    private readonly IBattleshipGameService _gameService;

    public BattleshipGameCreateController(IBattleshipGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        // En una implementación completa aquí pasaríamos la lista de amigos sin partida activa
        return View("~/Views/Battleship/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGameViewModel model)
    {
        if (!ModelState.IsValid)
            return View("~/Views/Battleship/Create.cshtml", model);

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var result = await _gameService.CreateGameAsync(currentUserId, model.OpponentId);

        if (result.HasError || result.Data == null)
        {
            TempData["Error"] = result.Error ?? "Error al crear la partida.";
            return RedirectToAction("Index", "BattleshipGameList");
        }

        TempData["Success"] = "Partida iniciada. Por favor, coloca tus barcos.";
        return RedirectToAction("Setup", "BattleshipSetup", new { gameId = result.Data.Id });
    }
}
