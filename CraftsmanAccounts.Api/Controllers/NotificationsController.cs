// متحكم الإشعارات - جلب إشعارات المستخدم وتحديث حالة القراءة
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class NotificationsController : BaseApiController
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    /// <summary>جلب إشعارات المستخدم الحالي مع التقسيم</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] PagedRequest request)
    {
        var result = await _service.GetByUserAsync(GetUserId(), request);
        return Ok(result.Data);
    }

    /// <summary>جلب عدد الإشعارات غير المقروءة</summary>
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var result = await _service.GetUnreadCountAsync(GetUserId());
        return Ok(result.Data);
    }

    /// <summary>تحديد إشعار كمقروء</summary>
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await _service.MarkAsReadAsync(GetUserId(), id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>تحديد جميع الإشعارات كمقروءة</summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await _service.MarkAllAsReadAsync(GetUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
