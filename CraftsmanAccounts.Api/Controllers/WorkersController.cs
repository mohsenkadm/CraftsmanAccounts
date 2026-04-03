// متحكم العمال - إضافة وتعديل وحذف وعرض العمال مع إشعارات OneSignal
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class WorkersController : BaseApiController
{
    private readonly IWorkerService _service;
    private readonly IUserNotificationService _notify;
    public WorkersController(IWorkerService service, IUserNotificationService notify)
    {
        _service = service;
        _notify = notify;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok((await _service.GetAllAsync(GetUserId())).Data);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(GetUserId(), id);
        return result.Success ? Ok(result.Data) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkerRequest request)
    {
        var result = await _service.CreateAsync(GetUserId(), request);
        if (result.Success)
        {
            await _notify.NotifyUserAsync(GetUserId(), "إضافة عامل", $"تم إضافة العامل {request.Name} بنجاح", "success");
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        return BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkerRequest request)
    {
        var result = await _service.UpdateAsync(GetUserId(), id, request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "تحديث عامل", "تم تحديث بيانات العامل بنجاح", "info");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "حذف عامل", "تم حذف العامل بنجاح", "warning");
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
