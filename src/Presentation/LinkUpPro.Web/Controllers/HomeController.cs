using AutoMapper;
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
    private readonly IMapper _mapper;

    public HomeController(IPostQueryService postQueryService, IMapper mapper)
    {
        _postQueryService = postQueryService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(string filter = "all")
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _postQueryService.GetFeedAsync(currentUserId);

        var postDtos = result.HasError || result.Data == null
            ? new List<LinkUpPro.Application.DTOs.Post.PostDto>()
            : result.Data.ToList();

        if (filter == "friends")
            postDtos = postDtos.Where(p => p.UserId != currentUserId).ToList();
        else if (filter == "mine")
            postDtos = postDtos.Where(p => p.UserId == currentUserId).ToList();

        var viewModels = _mapper.Map<List<PostViewModel>>(postDtos);

        // Campos calculados que AutoMapper ignora (configurado con opt.Ignore())
        foreach (var (vm, dto) in viewModels.Zip(postDtos))
        {
            vm.IsOwner = dto.UserId == currentUserId;
            vm.TimeAgo = $"{(int)(DateTime.UtcNow - dto.CreatedAt).TotalHours}h";
        }

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
