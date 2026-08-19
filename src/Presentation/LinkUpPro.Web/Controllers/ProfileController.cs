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

        var feedResult = await _postQueryService.GetUserPostsAsync(targetId, currentUserId);
        var postDtos = feedResult.HasError || feedResult.Data == null
            ? new List<LinkUpPro.Application.DTOs.Post.PostDto>()
            : feedResult.Data.ToList();

        var userPosts = _mapper.Map<List<PostViewModel>>(postDtos);
        foreach (var (vm, dto) in userPosts.Zip(postDtos))
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

        var model = new UpdateProfileViewModel
        {
            FirstName = profileResult.Data.FirstName,
            LastName = profileResult.Data.LastName,
            PhoneNumber = profileResult.Data.PhoneNumber,
            CurrentProfilePicture = profileResult.Data.ProfilePictureUrl
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProfileViewModel model)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());

        if (!ModelState.IsValid) 
        {
            var profileResult = await _userService.GetProfileAsync(currentUserId);
            if (profileResult.Data != null)
            {
                model.CurrentProfilePicture = profileResult.Data.ProfilePictureUrl;
            }
            return View(model);
        }

        // Si intenta cambiar la contraseña, validar que haya enviado la actual y la nueva
        bool intentandoCambiarPassword = !string.IsNullOrWhiteSpace(model.NewPassword) || !string.IsNullOrWhiteSpace(model.CurrentPassword);
        if (intentandoCambiarPassword)
        {
            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Debe proporcionar su contraseña actual para cambiarla.");
            }
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "Debe proporcionar una nueva contraseña.");
            }
            
            if (!ModelState.IsValid)
            {
                var profileResult = await _userService.GetProfileAsync(currentUserId);
                if (profileResult.Data != null)
                {
                    model.CurrentProfilePicture = profileResult.Data.ProfilePictureUrl;
                }
                return View(model);
            }
        }

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

            if (intentandoCambiarPassword)
            {
                var passDto = new LinkUpPro.Application.DTOs.User.ChangePasswordDto
                {
                    CurrentPassword = model.CurrentPassword!,
                    NewPassword = model.NewPassword!
                };
                var passResult = await _userService.ChangePasswordAsync(currentUserId, passDto);
                if (!passResult.Success)
                {
                    foreach (var error in passResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    var profileResult = await _userService.GetProfileAsync(currentUserId);
                    if (profileResult.Data != null)
                    {
                        model.CurrentProfilePicture = profileResult.Data.ProfilePictureUrl;
                    }
                    return View(model);
                }
            }

            TempData["Success"] = "Perfil actualizado exitosamente.";
            return RedirectToAction(nameof(View));
        }

        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? "Error al actualizar perfil.");
        var fallbackProfileResult = await _userService.GetProfileAsync(currentUserId);
        if (fallbackProfileResult.Data != null)
        {
            model.CurrentProfilePicture = fallbackProfileResult.Data.ProfilePictureUrl;
        }
        return View(model);
    }
}
