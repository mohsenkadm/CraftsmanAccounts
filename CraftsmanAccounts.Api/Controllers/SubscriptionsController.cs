// متحكم الاشتراكات - جلب أنواع الاشتراكات واشتراكات المستخدم وإنشاء اشتراك جديد
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class SubscriptionsController : BaseApiController
{
    private readonly IUserSubscriptionService _subscriptionService;
    private readonly ISubscriptionTypeService _subscriptionTypeService;
    private readonly IUserNotificationService _notify;

    public SubscriptionsController(
        IUserSubscriptionService subscriptionService,
        ISubscriptionTypeService subscriptionTypeService,
        IUserNotificationService notify)
    {
        _subscriptionService = subscriptionService;
        _subscriptionTypeService = subscriptionTypeService;
        _notify = notify;
    }

    /// <summary>جلب أنواع الاشتراكات المتاحة</summary>
    [HttpGet("types")]
    public async Task<IActionResult> GetTypes()
    {
        var result = await _subscriptionTypeService.GetAllActiveAsync();
        return Ok(result.Data);
    }

    /// <summary>جلب جميع اشتراكات المستخدم الحالي</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMySubscriptions()
    {
        var result = await _subscriptionService.GetMySubscriptionsAsync(GetUserId());
        return Ok(result.Data);
    }

    /// <summary>جلب الاشتراك الفعّال الحالي</summary>
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentSubscription()
    {
        var result = await _subscriptionService.GetCurrentSubscriptionAsync(GetUserId());
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    /// <summary>إنشاء اشتراك جديد</summary>
    [HttpPost]
    public async Task<IActionResult> Subscribe([FromBody] CreateUserSubscriptionRequest request)
    {
        var result = await _subscriptionService.SubscribeAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "اشتراك جديد", "تم تقديم طلب اشتراك جديد بنجاح وبانتظار الموافقة", "info");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
}
