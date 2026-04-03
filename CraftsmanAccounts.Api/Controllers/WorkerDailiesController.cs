// متحكم اليوميات - سجل يومي لأجور العمال ودفع الدفعات مع إشعارات OneSignal
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class WorkerDailiesController : BaseApiController
{
    private readonly IWorkerDailyService _service;
    private readonly IUserNotificationService _notify;
    public WorkerDailiesController(IWorkerDailyService service, IUserNotificationService notify)
    {
        _service = service;
        _notify = notify;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? projectId, [FromQuery] AccountStatementRequest request)
    {
        var result = await _service.GetAllAsync(GetUserId(), projectId, request);
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkerDailyRequest request)
    {
        var result = await _service.CreateAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "سجل يومي", "تم إنشاء سجل يومي جديد بنجاح", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpPost("pay-batch")]
    public async Task<IActionResult> PayBatch([FromBody] PayBatchRequest request)
    {
        var result = await _service.PayBatchAsync(GetUserId(), request.WorkerDailyIds, request.WalletId);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "دفع يوميات", $"تم دفع {request.WorkerDailyIds.Count} يومية بنجاح", "success");
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "حذف يومية", "تم حذف اليومية بنجاح", "warning");
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

// طلب دفع دفعة يوميات متعددة
public record PayBatchRequest(List<int> WorkerDailyIds, int WalletId);
