using AutoMapper;
using LinkUpPro.Application.DTOs.User;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Application.Interfaces.Post;
using LinkUpPro.Application.Interfaces.User;
using LinkUpPro.Application.ViewModels.Post;
using LinkUpPro.Application.ViewModels.User;
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
    private readonly IMapper _mapper;

    public ProfileController(
        IUserService userService,
        IPostQueryService postQueryService,
        IFriendshipService friendshipService,
        IMutualFriendService mutualFriendService,
        IMapper mapper)
    {
        _userService = userService;
        _postQueryService = postQueryService;
        _friendshipService = friendshipService;
        _mutualFriendService = mutualFriendService;
        _mapper = mapper;
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

        var filteredDtos = postDtos.Where(p => p.UserId == targetId).ToList();
        var userPosts = _mapper.Map<List<PostViewModel>>(filteredDtos);
        foreach (var (vm, dto) in userPosts.Zip(filteredDtos))
        {
            vm.IsOwner = dto.UserId == currentUserId;
            vm.TimeAgo = $"{(int)(DateTime.UtcNow - dto.CreatedAt).TotalHours}h";
        }

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

        // Mapear UserProfileDto → UpdateProfileViewModel (campos editables solamente)
        var model = new UpdateProfileViewModel
        {
            FirstName = profileResult.Data.FirstName,
            LastName = profileResult.Data.LastName,
            PhoneNumber = profileResult.Data.PhoneNumber
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());

        // Mapear ViewModel → DTO para el servicio
        var dto = _mapper.Map<UpdateProfileDto>(model);
        var result = await _userService.UpdateProfileAsync(currentUserId, dto);

        if (result.Success)
        {
            if (model.ProfilePicture != null)
            {
                var stream = model.ProfilePicture.OpenReadStream();
                await _userService.ChangeProfilePictureAsync(
                    currentUserId,
                    stream,
                    model.ProfilePicture.ContentType,
                    model.ProfilePicture.FileName);
            }

            TempData["Success"] = "Perfil actualizado exitosamente.";
            return RedirectToAction(nameof(View));
        }

        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? "Error al actualizar perfil.");
        return View(model);
    }
}
