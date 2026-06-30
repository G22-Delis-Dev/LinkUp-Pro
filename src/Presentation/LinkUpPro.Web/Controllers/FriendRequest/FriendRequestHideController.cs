using LinkUpPro.Application.Interfaces.Friendship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendRequestHideController : Controller
{
    private readonly IFriendRequestService _requestService;

    public FriendRequestHideController(IFriendRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Hide(Guid requestId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _requestService.HideRequestAsync(requestId, currentUserId);

        if (result.Success)
            TempData["Success"] = "Solicitud eliminada del historial.";
        else
            TempData["Error"] = result.Errors.FirstOrDefault() ?? "Error al procesar la solicitud.";

        return RedirectToAction("Index", "FriendRequestList", new { tab = "sent" });
    }
}
