using LinkUpPro.Application.DTOs.Friendship;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Application.Interfaces.User;
using LinkUpPro.Application.ViewModels.Friendship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendRequestSendController : Controller
{
    private readonly IFriendRequestService _requestService;
    private readonly IFriendRequestQueryService _queryService;
    private readonly IFriendshipService _friendshipService;
    private readonly IUserService _userService;

    public FriendRequestSendController(
        IFriendRequestService requestService,
        IFriendRequestQueryService queryService,
        IFriendshipService friendshipService,
        IUserService userService)
    {
        _requestService = requestService;
        _queryService = queryService;
        _friendshipService = friendshipService;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("~/Views/FriendRequest/Create.cshtml");
    }

    /// <summary>
    /// Endpoint AJAX para búsqueda global de usuarios en tiempo real.
    /// GET /FriendRequestSend/SearchUsers?q=texto
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> SearchUsers(string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Json(new List<object>());

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var results = await _userService.SearchUsersAsync(q, currentUserId, excludeFriendsAndPending: true);

        return Json(results.Select(u => new
        {
            id = u.Id,
            fullName = u.FullName,
            profilePictureUrl = u.ProfilePictureUrl
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(SendFriendRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/FriendRequest/Create.cshtml", model);
        }

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var dto = new SendFriendRequestDto
        {
            SenderId = currentUserId,
            ReceiverId = model.ReceiverId
        };

        var result = await _requestService.SendRequestAsync(dto);

        if (!result.HasError)
        {
            TempData["Success"] = "Solicitud de amistad enviada.";
            return RedirectToAction("Index", "FriendRequestList", new { tab = "sent" });
        }

        TempData["Error"] = result.Error ?? "Ocurrió un error al enviar la solicitud.";
        return RedirectToAction("Create");
    }
}
