using LinkUpPro.Application.DTOs.User;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Application.Interfaces.Post;
using LinkUpPro.Application.Interfaces.User;
using LinkUpPro.Application.ViewModels.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IUserService _userService;
    private readonly IPostQueryService _postQueryService;
    private readonly IFriendshipService _friendshipService;
    private readonly IMutualFriendService _mutualFriendService;

    public ProfileController(
        IUserService userService,
        IPostQueryService postQueryService,
        IFriendshipService friendshipService,
        IMutualFriendService mutualFriendService)
    {
        _userService = userService;
        _postQueryService = postQueryService;
        _friendshipService = friendshipService;
        _mutualFriendService = mutualFriendService;
    }

    [HttpGet]
    public async Task<IActionResult> View(Guid? id)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var targetId = id ?? currentUserId;
        var isOwner = currentUserId == targetId;

        var profileResult = await _userService.GetProfileAsync(targetId);
        if (profileResult.HasError || profileResult.Data == null) return NotFound();

        var feedResult = await _postQueryService.GetFeedAsync(targetId);
        var postDtos = feedResult.HasError || feedResult.Data == null 
            ? new List<LinkUpPro.Application.DTOs.Post.PostDto>() 
            : feedResult.Data.ToList();

        // Filtrar posts propios del usuario para su muro
        var userPosts = postDtos.Where(p => p.UserId == targetId).Select(p => new PostViewModel
        {
            Id = p.Id,
            UserId = p.UserId,
            AuthorName = p.AuthorName,
            AuthorProfilePicture = p.AuthorProfilePicture,
            Content = p.Content,
            Privacy = p.Privacy,
            ContentType = p.ContentType,
            AllowComments = p.AllowComments,
            ImageUrl = p.ImageUrl,
            YouTubeVideoId = p.YouTubeVideoId,
            CommentCount = p.CommentCount,
            LikeCount = p.LikeCount,
            DislikeCount = p.DislikeCount,
            CreatedAt = p.CreatedAt,
            TimeAgo = $"{(int)(DateTime.UtcNow - p.CreatedAt).TotalHours}h",
            IsOwner = p.UserId == currentUserId
        }).ToList();

        ViewBag.IsOwner = isOwner;
        ViewBag.Posts = userPosts;
        
        var friends = await _friendshipService.GetFriendsAsync(targetId);
        ViewBag.FriendsCount = friends.Count;

        if (!isOwner)
        {
            var areFriends = await _friendshipService.AreFriendsAsync(currentUserId, targetId);
            ViewBag.AreFriends = areFriends;
            
            var mutualFriends = await _mutualFriendService.GetMutualFriendsAsync(currentUserId, targetId);
            ViewBag.MutualFriendsCount = mutualFriends.Count;
        }

        return View(profileResult.Data);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var profileResult = await _userService.GetProfileAsync(currentUserId);
        
        if (profileResult.HasError || profileResult.Data == null) return NotFound();

        var model = new UpdateProfileDto
        {
            FirstName = profileResult.Data.FirstName,
            LastName = profileResult.Data.LastName,
            PhoneNumber = profileResult.Data.PhoneNumber
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateProfileDto model, IFormFile? profilePicture)
    {
        if (!ModelState.IsValid) return View(model);

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var result = await _userService.UpdateProfileAsync(currentUserId, model);
        
        if (result.Success)
        {
            if (profilePicture != null)
            {
                var stream = profilePicture.OpenReadStream();
                await _userService.ChangeProfilePictureAsync(currentUserId, stream, profilePicture.ContentType, profilePicture.FileName);
            }

            TempData["Success"] = "Perfil actualizado exitosamente.";
            return RedirectToAction(nameof(View));
        }

        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? "Error al actualizar perfil.");
        return View(model);
    }
}
