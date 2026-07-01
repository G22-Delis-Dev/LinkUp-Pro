using LinkUpPro.Application.Interfaces.Battleship;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Application.ViewModels.Battleship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class BattleshipGameCreateController : Controller
{
    private readonly IBattleshipGameService _gameService;
    private readonly IFriendshipService _friendshipService;

    public BattleshipGameCreateController(
        IBattleshipGameService gameService,
        IFriendshipService friendshipService)
    {
        _gameService = gameService;
        _friendshipService = friendshipService;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var currentUserId = GetCurrentUserId();
        var friends = await _friendshipService.GetFriendsAsync(currentUserId);
        ViewBag.Friends = friends;
        return View("~/Views/Battleship/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGameViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var currentUserId2 = GetCurrentUserId();
            ViewBag.Friends = await _friendshipService.GetFriendsAsync(currentUserId2);
            return View("~/Views/Battleship/Create.cshtml", model);
        }

        var currentUserId = GetCurrentUserId();
        var result = await _gameService.CreateGameAsync(currentUserId, model.OpponentId);

        if (result.HasError || result.Data == null)
        {
            TempData["Error"] = result.Error ?? "Error al crear la partida.";
            return RedirectToAction("Index", "BattleshipGameList");
        }

        TempData["Success"] = "Partida iniciada. Por favor, coloca tus barcos.";
        return RedirectToAction("Setup", "BattleshipSetup", new { gameId = result.Data.Id });
    }

    private Guid GetCurrentUserId()
        => Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
}
