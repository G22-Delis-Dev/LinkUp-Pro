using LinkUpPro.Application.Interfaces.Friendship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendRequestListController : Controller
{
    private readonly IFriendRequestQueryService _queryService;

    public FriendRequestListController(IFriendRequestQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<IActionResult> Index(string tab = "received")
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        ViewBag.ActiveTab = tab;
        
        if (tab == "sent")
        {
            var sentRequests = await _queryService.GetSentRequestsAsync(currentUserId);
            return View("~/Views/FriendRequest/Index.cshtml", sentRequests);
        }
        else
        {
            var receivedRequests = await _queryService.GetReceivedRequestsAsync(currentUserId);
            return View("~/Views/FriendRequest/Index.cshtml", receivedRequests);
        }
    }
}
