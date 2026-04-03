// متحكم سندات القبض - إنشاء سندات قبض عامة ولمشاريع مع إشعارات OneSignal
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class ReceiptsController : BaseApiController
{
    private readonly IReceiptService _service;
    private readonly IUserNotificationService _notify;
    public ReceiptsController(IReceiptService service, IUserNotificationService notify)
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
    public async Task<IActionResult> CreateGeneral([FromBody] CreateReceiptGeneralRequest request)
    {
        var result = await _service.CreateGeneralAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "سند قبض", $"تم إنشاء سند قبض عام بمبلغ {request.Amount}", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpPost("project")]
    public async Task<IActionResult> CreateProject([FromBody] CreateReceiptProjectRequest request)
    {
        var result = await _service.CreateProjectAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "سند قبض مشروع", $"تم إنشاء سند قبض للمشروع بمبلغ {request.Amount}", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "حذف سند قبض", "تم حذف سند القبض بنجاح", "warning");
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
