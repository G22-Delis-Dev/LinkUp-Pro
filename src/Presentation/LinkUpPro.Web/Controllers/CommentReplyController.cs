using LinkUpPro.Application.Interfaces.Comment;
using LinkUpPro.Application.ViewModels.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class CommentReplyController : Controller
{
    private readonly ICommentReplyService _commentReplyService;

    public CommentReplyController(ICommentReplyService commentReplyService)
    {
        _commentReplyService = commentReplyService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCommentReplyViewModel model, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
            
            var dto = new LinkUpPro.Application.DTOs.Comment.CreateCommentReplyDto
            {
                CommentId = model.CommentId,
                UserId = currentUserId,
                Content = model.Content
            };

            var result = await _commentReplyService.CreateReplyAsync(dto);
            
            if (!result.HasError)
                TempData["Success"] = "Respuesta agregada.";
            else
                TempData["Error"] = result.Error;
        }
        else
        {
            TempData["Error"] = "La respuesta no es válida.";
        }

        return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string returnUrl)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _commentReplyService.DeleteReplyAsync(id, currentUserId);
        
        if (result.Success)
            TempData["Success"] = "Respuesta eliminada.";
        else
            TempData["Error"] = result.Errors.FirstOrDefault() ?? "Ocurrió un error al eliminar la respuesta.";
            
        return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
    }
}
