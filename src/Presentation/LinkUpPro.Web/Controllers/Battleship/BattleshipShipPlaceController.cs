using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Application.ViewModels.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipShipPlaceController : Controller
{
    private readonly IBattleshipSetupService _setupService;

    public BattleshipShipPlaceController(IBattleshipSetupService setupService)
    {
        _setupService = setupService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Place(PlaceShipViewModel model)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var dto = new PlaceShipDto
        {
            GameId = model.GameId,
            PlayerId = currentUserId,
            Size = model.Size,
            Direction = model.Direction,
            StartX = model.StartX,
            StartY = model.StartY
        };

        var result = await _setupService.PlaceShipAsync(dto);

        if (result.HasError)
        {
            return Json(new { success = false, message = result.Error });
        }

        return Json(new { success = true, ship = result.Data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmSetup(Guid gameId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _setupService.ConfirmSetupAsync(gameId, currentUserId);

        if (result.Success)
        {
            TempData["Success"] = "Barcos confirmados. Esperando al oponente...";
            return RedirectToAction("Setup", "BattleshipSetup", new { gameId });
        }
        
        TempData["Error"] = result.Errors.FirstOrDefault() ?? "Error al confirmar barcos.";
        return RedirectToAction("Setup", "BattleshipSetup", new { gameId });
    }
}
