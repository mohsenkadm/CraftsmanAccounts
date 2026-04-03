// متحكم لوحة التحكم - عرض الإحصائيات العامة للمدير
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CraftsmanAccounts.Web.Models.ViewModels;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IUserService _userService;
    private readonly IUserSubscriptionService _subscriptionService;

    public DashboardController(IUserService userService, IUserSubscriptionService subscriptionService)
    {
        _userService = userService;
        _subscriptionService = subscriptionService;
    }

    public IActionResult Index()
    {
        var model = new DashboardViewModel
        {
            TotalUsers = _userService.GetTotalCount(),
            ActiveUsers = _userService.GetActiveCount(),
            PendingApprovals = _userService.GetPendingCount(),
            TotalSubscriptions = _subscriptionService.GetTotalCount(),
            ActiveSubscriptions = _subscriptionService.GetActiveCount(),
            TotalRevenue = _subscriptionService.GetTotalRevenue(),
            RecentSubscriptions = _subscriptionService.GetRecent(5)
        };

        return View(model);
    }
}
