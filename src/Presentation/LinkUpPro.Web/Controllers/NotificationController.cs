using LinkUpPro.Application.Interfaces.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUpPro.Web.Controllers;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var notifications = await _notificationService.GetNotificationsAsync(currentUserId);
        
        // Mostrar todas las notificaciones ordenadas de más reciente a más antigua
        var orderedNotifications = notifications.OrderByDescending(n => n.CreatedAt).ToList();
        
        // Contador de no leídas para el badge
        ViewBag.UnreadCount = orderedNotifications.Count(n => !n.IsRead);
        
        return View(orderedNotifications);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _notificationService.MarkAsReadAsync(id, currentUserId);
        
        return Json(new { success = result.Success });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var currentUserId = Guid.Parse(User.FindFirst("uid")?.Value ?? Guid.Empty.ToString());
        var result = await _notificationService.MarkAllAsReadAsync(currentUserId);
        
        if (result.Success)
        {
            TempData["Success"] = "Todas las notificaciones han sido marcadas como leídas.";
        }
        else
        {
            TempData["Error"] = "Error al marcar las notificaciones.";
        }

        return RedirectToAction(nameof(Index));
    }
}
