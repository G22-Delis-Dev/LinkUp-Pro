using LinkUpPro.Application.Interfaces.Friendship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendRequestRejectController : Controller
{
    private readonly IFriendRequestService _requestService;

    public FriendRequestRejectController(IFriendRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid requestId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _requestService.RejectRequestAsync(requestId, currentUserId);

        if (result.Success)
            TempData["Success"] = "Solicitud rechazada.";
        else
            TempData["Error"] = result.Errors.FirstOrDefault() ?? "Error al procesar la solicitud.";

        return RedirectToAction("Index", "FriendRequestList");
    }
}
