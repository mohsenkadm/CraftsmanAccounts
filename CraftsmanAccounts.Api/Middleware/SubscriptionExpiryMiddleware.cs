// ميدل وير التحقق من اشتراك المستخدم - يمنع الوصول إذا انتهى الاشتراك
using System.Security.Claims;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Api.Middleware;

public class SubscriptionExpiryMiddleware
{
    private readonly RequestDelegate _next;

    // المسارات المستثناة من التحقق (تسجيل الدخول والتسجيل والتحقق)
    private static readonly string[] ExcludedPaths = ["/api/userauth/login", "/api/userauth/register", "/api/userauth/send-otp", "/api/userauth/verify-otp", "/api/userauth/reset-password", "/swagger"];

    public SubscriptionExpiryMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, IUnitOfWork uow)
    {
        // تجاوز المسارات المستثناة
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (ExcludedPaths.Any(p => path.StartsWith(p)))
        {
            await _next(context);
            return;
        }

        // تجاوز الطلبات غير المصادق عليها
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            await _next(context);
            return;
        }

        // التحقق من وجود اشتراك فعال
        var hasActive = await uow.Repository<UserSubscription>().Query()
            .AnyAsync(s => s.UserId == userId
                && (s.Status == SubscriptionStatus.Approved || s.Status == SubscriptionStatus.Paid)
                && s.EndDate >= DateTime.UtcNow);

        if (!hasActive)
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsync("{\"success\":false,\"message\":\"اشتراكك منتهي. يرجى تجديد الاشتراك للمتابعة.\"}");
            return;
        }

        await _next(context);
    }
}
