using LinkUpPro.Application.Interfaces.Friendship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendDeleteController : Controller
{
    private readonly IFriendshipService _friendshipService;

    public FriendDeleteController(IFriendshipService friendshipService)
    {
        _friendshipService = friendshipService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid friendId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var result = await _friendshipService.RemoveFriendAsync(currentUserId, friendId);
        
        if (result.Success)
        {
            TempData["Success"] = "Amigo eliminado correctamente.";
        }
        else
        {
            TempData["Error"] = result.Errors.FirstOrDefault() ?? "Ocurrió un error al procesar la solicitud. Inténtelo nuevamente.";
        }

        return RedirectToAction("Index", "FriendList");
    }
}
