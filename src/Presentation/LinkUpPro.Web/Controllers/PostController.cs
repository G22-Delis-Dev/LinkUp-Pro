using AutoMapper;
using LinkUpPro.Application.DTOs.Post;
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
    private readonly IMapper _mapper;

    public PostController(IPostService postService, IPostQueryService postQueryService, IMapper mapper)
    {
        _postService = postService;
        _postQueryService = postQueryService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePostViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePostViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());

        var dto = _mapper.Map<CreatePostDto>(model);
        dto.UserId = currentUserId;

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditPostViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());

        var dto = _mapper.Map<UpdatePostDto>(model);
        dto.UserId = currentUserId;

        var result = await _postService.UpdatePostAsync(dto);

        if (result.Success)
        {
            TempData["Success"] = "Publicación actualizada.";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? "No se pudo actualizar la publicación. Inténtalo de nuevo.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _postService.DeletePostAsync(id, currentUserId);

        if (result.Success)
            TempData["Success"] = "Publicación eliminada.";
        else
            TempData["Error"] = result.Errors.FirstOrDefault() ?? "No se pudo eliminar la publicación. Inténtalo de nuevo.";

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _postQueryService.GetPostByIdAsync(id, currentUserId);
        
        if (result.HasError || result.Data == null) return NotFound();

        var viewModel = _mapper.Map<PostViewModel>(result.Data);
        viewModel.IsOwner = result.Data.UserId == currentUserId;
        viewModel.TimeAgo = $"{(int)(DateTime.UtcNow - result.Data.CreatedAt).TotalHours}h";

        return View(viewModel);
    }
}
