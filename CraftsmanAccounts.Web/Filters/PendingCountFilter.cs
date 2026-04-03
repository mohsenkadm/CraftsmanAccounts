// فلتر عدد الطلبات المعلقة - يضيف عدد المستخدمين المعلقين إلى كل صفحة تلقائياً
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Filters;

/// <summary>
/// فلتر إجراء يحسب عدد الطلبات المعلقة ويمررها عبر ViewBag
/// </summary>
public class PendingCountFilter : IActionFilter
{
    private readonly IUserService _userService;

    public PendingCountFilter(IUserService userService)
    {
        _userService = userService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is Controller controller && context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            controller.ViewBag.PendingCount = _userService.GetPendingCount();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
