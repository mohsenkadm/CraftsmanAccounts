// متحكم موافقات المستخدمين - قبول ورفض طلبات التسجيل المعلقة
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CraftsmanAccounts.Web.Hubs;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Controllers;

[Authorize]
public class UserApprovalsController : Controller
{
    private readonly IUserService _userService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public UserApprovalsController(IUserService userService, IHubContext<NotificationHub> hubContext)
    {
        _userService = userService;
        _hubContext = hubContext;
    }

    public IActionResult Index()
    {
        var pendingUsers = _userService.GetPendingUsers();
        return View(pendingUsers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var user = _userService.GetById(id);
        _userService.Approve(id);

        await _hubContext.Clients.All.SendAsync("ReceiveNotification",
            "قبول مستخدم",
            $"تم قبول المستخدم {user?.FullName} بنجاح",
            "success");

        TempData["Success"] = $"تم قبول المستخدم {user?.FullName} بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var user = _userService.GetById(id);
        _userService.Reject(id);

        await _hubContext.Clients.All.SendAsync("ReceiveNotification",
            "رفض مستخدم",
            $"تم رفض المستخدم {user?.FullName}",
            "danger");

        TempData["Success"] = $"تم رفض المستخدم {user?.FullName}";
        return RedirectToAction(nameof(Index));
    }
}
