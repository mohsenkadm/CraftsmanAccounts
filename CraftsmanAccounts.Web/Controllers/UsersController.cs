// متحكم المستخدمين - عرض وتعديل وحذف وتفعيل/إلغاء تفعيل المستخدمين
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CraftsmanAccounts.Web.Hubs;
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Models.ViewModels;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public UsersController(IUserService userService, IHubContext<NotificationHub> hubContext)
    {
        _userService = userService;
        _hubContext = hubContext;
    }

    public IActionResult Index(UserFilterViewModel filter)
    {
        var result = _userService.GetUsers(filter);
        return View(result);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var user = _userService.GetById(id);
        if (user is null) return NotFound();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(AppUser user)
    {
        if (!ModelState.IsValid) return View(user);

        _userService.Update(user);
        TempData["Success"] = "تم تحديث بيانات المستخدم بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        _userService.Delete(id);
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", "حذف مستخدم", "تم حذف المستخدم بنجاح", "warning");
        TempData["Success"] = "تم حذف المستخدم بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        _userService.ToggleActive(id);
        var user = _userService.GetById(id);
        var status = user?.IsActive == true ? "تفعيل" : "إلغاء تفعيل";
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"{status} مستخدم", $"تم {status} المستخدم بنجاح", "info");
        TempData["Success"] = $"تم {status} المستخدم بنجاح";
        return RedirectToAction(nameof(Index));
    }
}
