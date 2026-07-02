using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Application.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendProfileController : Controller
{
    private readonly IFriendshipService _friendshipService;
    private readonly IUserService _userService;

    public FriendProfileController(IFriendshipService friendshipService, IUserService userService)
    {
        _friendshipService = friendshipService;
        _userService = userService;
    }

    public async Task<IActionResult> View(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var areFriends = await _friendshipService.AreFriendsAsync(currentUserId, id);
        if (!areFriends && currentUserId != id)
        {
            TempData["Error"] = "No tienes permiso para ver este perfil.";
            return RedirectToAction("Index", "Home");
        }

        var profileResponse = await _userService.GetProfileAsync(id);
        if (profileResponse.HasError || profileResponse.Data == null)
        {
            return NotFound();
        }

        var postQueryService = HttpContext.RequestServices.GetRequiredService<LinkUpPro.Application.Interfaces.Post.IPostQueryService>();
        var mapper = HttpContext.RequestServices.GetRequiredService<AutoMapper.IMapper>();

        var feedResult = await postQueryService.GetUserPostsAsync(id, currentUserId);
        var postDtos = feedResult.HasError || feedResult.Data == null
            ? new List<LinkUpPro.Application.DTOs.Post.PostDto>()
            : feedResult.Data.ToList();

        var userPosts = mapper.Map<List<LinkUpPro.Application.ViewModels.Post.PostViewModel>>(postDtos);
        foreach (var (vm, dto) in userPosts.Zip(postDtos))
        {
            vm.IsOwner = dto.UserId == currentUserId;
            vm.TimeAgo = $"{(int)(DateTime.UtcNow - dto.CreatedAt).TotalHours}h";
        }

        ViewBag.Posts = userPosts;

        return View("~/Views/Friendship/FriendProfile.cshtml", profileResponse.Data);
    }
}
