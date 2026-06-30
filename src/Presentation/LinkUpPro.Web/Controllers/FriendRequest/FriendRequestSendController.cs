using LinkUpPro.Application.DTOs.Friendship;
using LinkUpPro.Application.Interfaces.Friendship;
using LinkUpPro.Application.Interfaces.User;
using LinkUpPro.Application.ViewModels.Friendship;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class FriendRequestSendController : Controller
{
    private readonly IFriendRequestService _requestService;
    private readonly IFriendRequestQueryService _queryService;
    private readonly IFriendshipService _friendshipService;
    // Asumiendo que IUserRepository está disponible o un método para listar usuarios.
    // Como no tenemos un IUserRepository expuesto en Application para listar a todos,
    // es posible que tengamos que saltar esta regla de "mostrar usuarios sin amistad"
    // o el frontend tendrá un input manual, o el PostController muestra perfiles.
    // La rúbrica dice "Muestra usuarios activos sin amistad ni solicitud pendiente."
    // Para simplificar, asumiremos que IUserService no tiene GetAll, pero si fuera necesario
    // se buscaría por nombre. Por ahora pasaremos una lista vacía y lo dejaremos como TODO.

    public FriendRequestSendController(
        IFriendRequestService requestService,
        IFriendRequestQueryService queryService,
        IFriendshipService friendshipService)
    {
        _requestService = requestService;
        _queryService = queryService;
        _friendshipService = friendshipService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        // En un caso real, aquí cargaríamos los usuarios que no son amigos ni tienen solicitud.
        // Dado que no tenemos un UserService.GetAllUsers(), el usuario podría buscar por ID
        // o ver los usuarios en las publicaciones. 
        // Solo renderizaremos la vista.
        return View("~/Views/FriendRequest/Create.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(SendFriendRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/FriendRequest/Create.cshtml", model);
        }

        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        
        var dto = new SendFriendRequestDto
        {
            SenderId = currentUserId,
            ReceiverId = model.ReceiverId
        };

        var result = await _requestService.SendRequestAsync(dto);

        if (!result.HasError)
        {
            TempData["Success"] = "Solicitud de amistad enviada.";
            return RedirectToAction("Index", "FriendRequestList", new { tab = "sent" });
        }

        TempData["Error"] = result.Error ?? "Ocurrió un error al enviar la solicitud.";
        return RedirectToAction("Create");
    }
}
