using LinkUpPro.Application.Interfaces.Comment;
using LinkUpPro.Application.ViewModels.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class CommentController : Controller
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCommentViewModel model, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
            
            var dto = new LinkUpPro.Application.DTOs.Comment.CreateCommentDto
            {
                PostId = model.PostId,
                UserId = currentUserId,
                Content = model.Content
            };

            var result = await _commentService.CreateCommentAsync(dto);
            
            if (!result.HasError)
                TempData["Success"] = "Comentario agregado.";
            else
                TempData["Error"] = result.Error;
        }
        else
        {
            TempData["Error"] = "El comentario no es válido.";
        }

        return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id, string returnUrl)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _commentService.DeleteCommentAsync(id, currentUserId);
        
        if (result.Success)
            TempData["Success"] = "Comentario eliminado.";
        else
            TempData["Error"] = result.Errors.FirstOrDefault() ?? "Ocurrió un error al eliminar el comentario.";
            
        return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
    }
}
