using LinkUpPro.Application.Interfaces.Reaction;
using LinkUpPro.Application.ViewModels.Reaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class ReactionController : Controller
{
    private readonly IReactionService _reactionService;

    public ReactionController(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    [HttpPost]
    public async Task<IActionResult> Toggle([FromBody] ToggleReactionViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var dto = new LinkUpPro.Application.DTOs.Reaction.ToggleReactionDto
        {
            PostId = model.PostId,
            UserId = currentUserId,
            Type = model.Type
        };
        
        var result = await _reactionService.ToggleReactionAsync(dto);

        if (result.HasError)
        {
            return Json(new { success = false, error = result.Error });
        }

        // Obtener conteos actualizados
        var counts = await _reactionService.GetReactionCountsAsync(model.PostId, currentUserId);
        
        return Json(new { 
            success = true, 
            likeCount = counts.LikeCount, 
            dislikeCount = counts.DislikeCount,
            userReaction = counts.UserReaction  // -1 = ninguna, 0 = Like, 1 = Dislike
        });
    }
}
