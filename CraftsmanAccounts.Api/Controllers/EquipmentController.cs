// متحكم المعدات - إضافة المعدات وتسجيل التلف مع إشعارات OneSignal
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class EquipmentController : BaseApiController
{
    private readonly IEquipmentService _service;
    private readonly IUserNotificationService _notify;
    public EquipmentController(IEquipmentService service, IUserNotificationService notify)
    {
        _service = service;
        _notify = notify;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok((await _service.GetAllAsync(GetUserId())).Data);

    // جلب المعدات المتاحة فقط (غير مشغولة وغير تالفة)
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable() => Ok((await _service.GetAvailableAsync(GetUserId())).Data);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentRequest request)
    {
        var result = await _service.CreateAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "إضافة معدة", $"تم إضافة المعدة {request.Name} بنجاح", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpPost("{id}/damage")]
    public async Task<IActionResult> MarkDamaged(int id)
    {
        var result = await _service.MarkDamagedAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "تلف معدة", "تم تسجيل تلف المعدة", "danger");
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "حذف معدة", "تم حذف المعدة بنجاح", "warning");
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // كشف المعدات الشامل (كمي ومالي) مع فلاتر
    [HttpGet("statement")]
    public async Task<IActionResult> GetStatement([FromQuery] EquipmentStatementRequest request)
    {
        var result = await _service.GetStatementAsync(GetUserId(), request);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
}
