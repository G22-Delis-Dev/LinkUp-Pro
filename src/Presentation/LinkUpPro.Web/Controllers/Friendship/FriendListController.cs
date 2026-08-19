using LinkUpPro.Application.Interfaces.Friendship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendListController : Controller
{
    private readonly IFriendshipService _friendshipService;
    private readonly IMutualFriendService _mutualFriendService;

    public FriendListController(IFriendshipService friendshipService, IMutualFriendService mutualFriendService)
    {
        _friendshipService = friendshipService;
        _mutualFriendService = mutualFriendService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var friends = await _friendshipService.GetFriendsAsync(currentUserId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            friends = friends.Where(f => f.FriendName.ToLower().Contains(search)).ToList();
        }

        // Orden alfabético
        friends = friends.OrderBy(f => f.FriendName).ToList();

        ViewBag.SearchQuery = search;
        return View("~/Views/Friendship/Index.cshtml", friends);
    }

    public async Task<IActionResult> MutualFriends(Guid friendId)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var mutualFriends = await _mutualFriendService.GetMutualFriendsAsync(currentUserId, friendId);
        
        return PartialView("~/Views/Friendship/_MutualFriends.cshtml", mutualFriends);
    }
}
