using LinkUpPro.Application.Interfaces.Friendship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendRequestCancelController : Controller
{
    private readonly IFriendRequestService _requestService;

    public FriendRequestCancelController(IFriendRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid requestId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _requestService.CancelRequestAsync(requestId, currentUserId);

        if (result.Success)
            TempData["Success"] = "Solicitud cancelada.";
        else
            TempData["Error"] = result.Errors.FirstOrDefault() ?? "Error al procesar la solicitud.";

        return RedirectToAction("Index", "FriendRequestList", new { tab = "sent" });
    }
}
