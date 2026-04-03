// فلتر تسجيل النشاطات تلقائياً - يسجل كل عملية POST/PUT/DELETE يقوم بها المستخدم
using System.Security.Claims;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CraftsmanAccounts.Api.Filters;

public class ActivityLogFilter : IAsyncActionFilter
{
    private readonly IActivityLogService _logger;

    public ActivityLogFilter(IActivityLogService logger) => _logger = logger;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var result = await next();

        // تسجيل فقط العمليات الناجحة (POST/PUT/DELETE)
        var method = context.HttpContext.Request.Method;
        if (method is not ("POST" or "PUT" or "DELETE")) return;
        if (result.Exception != null) return;

        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId)) return;

        var action = method switch { "POST" => "إضافة", "PUT" => "تعديل", "DELETE" => "حذف", _ => method };
        var controller = context.RouteData.Values["controller"]?.ToString() ?? "غير معروف";
        var entityId = context.RouteData.Values["id"] is string idStr && int.TryParse(idStr, out var id) ? id : (int?)null;
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        await _logger.LogAsync(userId, action, controller, entityId, $"{action} في {controller}", ip);
    }
}
