using LinkUpPro.Application.Interfaces.Post;
using LinkUpPro.Application.ViewModels.Post;
using LinkUpPro.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IPostQueryService _postQueryService;

    public HomeController(IPostQueryService postQueryService)
    {
        _postQueryService = postQueryService;
    }

    public async Task<IActionResult> Index(string filter = "all")
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _postQueryService.GetFeedAsync(currentUserId);
        
        var postDtos = result.HasError || result.Data == null 
            ? new List<LinkUpPro.Application.DTOs.Post.PostDto>() 
            : result.Data.ToList();

        if (filter == "friends")
        {
            postDtos = postDtos.Where(p => p.UserId != currentUserId).ToList();
        }
        else if (filter == "mine")
        {
            postDtos = postDtos.Where(p => p.UserId == currentUserId).ToList();
        }

        var viewModels = postDtos.Select(p => new PostViewModel
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

        ViewBag.CurrentFilter = filter;
        return View(viewModels);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

