// متحكم اشتراكات المستخدمين - إدارة الاشتراكات والموافقات وتسجيل الدفع
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CraftsmanAccounts.Web.Hubs;
using CraftsmanAccounts.Web.Models.ViewModels;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Controllers;

[Authorize]
public class UserSubscriptionsController : Controller
{
    private readonly IUserSubscriptionService _service;
    private readonly IHubContext<NotificationHub> _hubContext;

    public UserSubscriptionsController(IUserSubscriptionService service, IHubContext<NotificationHub> hubContext)
    {
        _service = service;
        _hubContext = hubContext;
    }

    public IActionResult Index(UserSubscriptionFilterViewModel filter)
    {
        var result = _service.GetSubscriptions(filter);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var sub = _service.GetById(id);
        _service.Approve(id);

        await _hubContext.Clients.All.SendAsync("ReceiveNotification",
            "قبول اشتراك",
            $"تم قبول اشتراك {sub?.UserName} بنجاح",
            "success");

        TempData["Success"] = "تم قبول الاشتراك بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var sub = _service.GetById(id);
        _service.Reject(id);

        await _hubContext.Clients.All.SendAsync("ReceiveNotification",
            "رفض اشتراك",
            $"تم رفض اشتراك {sub?.UserName}",
            "danger");

        TempData["Success"] = "تم رفض الاشتراك";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var sub = _service.GetById(id);
        _service.MarkAsPaid(id);

        await _hubContext.Clients.All.SendAsync("ReceiveNotification",
            "تسجيل دفع",
            $"تم تسجيل دفع اشتراك {sub?.UserName} بنجاح",
            "success");

        TempData["Success"] = "تم تسجيل الدفع بنجاح";
        return RedirectToAction(nameof(Index));
    }
}
