using LinkUpPro.Application.Interfaces.Post;
using LinkUpPro.Application.ViewModels.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class PostController : Controller
{
    private readonly IPostService _postService;
    private readonly IPostQueryService _postQueryService;

    public PostController(IPostService postService, IPostQueryService postQueryService)
    {
        _postService = postService;
        _postQueryService = postQueryService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePostViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePostViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var dto = new LinkUpPro.Application.DTOs.Post.CreatePostDto
        {
            UserId = currentUserId,
            Content = model.Content,
            Privacy = model.Privacy,
            ContentType = model.ContentType,
            AllowComments = model.AllowComments,
            YouTubeUrl = model.YouTubeUrl
        };

        if (model.Image != null)
        {
            dto.ImageStream = model.Image.OpenReadStream();
            dto.ImageContentType = model.Image.ContentType;
            dto.ImageFileName = model.Image.FileName;
        }

        var result = await _postService.CreatePostAsync(dto);

        if (!result.HasError)
        {
            TempData["Success"] = "Publicación creada exitosamente.";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "Error al crear la publicación.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _postQueryService.GetPostByIdAsync(id, currentUserId);
        
        if (result.HasError || result.Data == null) return NotFound();
        if (result.Data.UserId != currentUserId) return Forbid();

        var model = new EditPostViewModel
        {
            Id = result.Data.Id,
            Content = result.Data.Content,
            Privacy = result.Data.Privacy,
            AllowComments = result.Data.AllowComments
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditPostViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var dto = new LinkUpPro.Application.DTOs.Post.UpdatePostDto
        {
            PostId = model.Id,
            UserId = currentUserId,
            Content = model.Content,
            Privacy = model.Privacy,
            AllowComments = model.AllowComments
        };

        var result = await _postService.UpdatePostAsync(dto);

        if (result.Success)
        {
            TempData["Success"] = "Publicación actualizada.";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? "Error");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _postService.DeletePostAsync(id, currentUserId);

        if (result.Success)
            TempData["Success"] = "Publicación eliminada.";
        else
            TempData["Error"] = result.Errors.FirstOrDefault();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _postQueryService.GetPostByIdAsync(id, currentUserId);
        
        if (result.HasError || result.Data == null) return NotFound();

        var p = result.Data;
        var viewModel = new PostViewModel
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
        };

        return View(viewModel);
    }
}
