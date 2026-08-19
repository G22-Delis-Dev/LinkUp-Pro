using LinkUpPro.Application.Interfaces.Friendship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendRequestAcceptController : Controller
{
    private readonly IFriendRequestService _requestService;

    public FriendRequestAcceptController(IFriendRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(Guid requestId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _requestService.AcceptRequestAsync(requestId, currentUserId);

        if (result.Success)
            TempData["Success"] = "Solicitud de amistad aceptada.";
        else
            TempData["Error"] = result.Errors.FirstOrDefault() ?? "Error al procesar la solicitud.";

        return RedirectToAction("Index", "FriendRequestList");
    }
}
