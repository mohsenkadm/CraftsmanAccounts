// متحكم سندات الصرف - إنشاء مدفوعات متنوعة مع إشعارات OneSignal
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class PaymentsController : BaseApiController
{
    private readonly IPaymentService _service;
    private readonly IUserNotificationService _notify;
    public PaymentsController(IPaymentService service, IUserNotificationService notify)
    {
        _service = service;
        _notify = notify;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AccountStatementRequest request)
    {
        var result = await _service.GetAllAsync(GetUserId(), request);
        return Ok(result.Data);
    }

    [HttpPost("general")]
    public async Task<IActionResult> CreateGeneral([FromBody] CreatePaymentGeneralRequest request)
    {
        var result = await _service.CreateGeneralAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "سند صرف", $"تم إنشاء سند صرف عام بمبلغ {request.Amount}", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpPost("project")]
    public async Task<IActionResult> CreateProject([FromBody] CreatePaymentProjectRequest request)
    {
        var result = await _service.CreateProjectAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "سند صرف مشروع", $"تم إنشاء سند صرف للمشروع بمبلغ {request.Amount}", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpPost("client")]
    public async Task<IActionResult> CreateClient([FromBody] CreatePaymentClientRequest request)
    {
        var result = await _service.CreateClientAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "سند صرف عميل", $"تم إنشاء سند صرف للعميل بمبلغ {request.Amount}", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpPost("worker")]
    public async Task<IActionResult> CreateWorker([FromBody] CreatePaymentWorkerRequest request)
    {
        var result = await _service.CreateWorkerAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "سند صرف عامل", $"تم إنشاء سند صرف للعامل بمبلغ {request.Amount}", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "حذف سند صرف", "تم حذف سند الصرف بنجاح", "warning");
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
