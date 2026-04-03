// متحكم العملاء - إدارة العملاء مع فلترة حسب النوع وإشعارات OneSignal
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class ClientsController : BaseApiController
{
    private readonly IClientService _service;
    private readonly IUserNotificationService _notify;
    public ClientsController(IClientService service, IUserNotificationService notify)
    {
        _service = service;
        _notify = notify;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ClientType? type) => Ok((await _service.GetAllAsync(GetUserId(), type)).Data);

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(GetUserId(), id);
        return result.Success ? Ok(result.Data) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        var result = await _service.CreateAsync(GetUserId(), request);
        if (result.Success)
        {
            await _notify.NotifyUserAsync(GetUserId(), "إضافة عميل", $"تم إضافة العميل {request.Name} بنجاح", "success");
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        return BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClientRequest request)
    {
        var result = await _service.UpdateAsync(GetUserId(), id, request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "تحديث عميل", "تم تحديث بيانات العميل بنجاح", "info");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "حذف عميل", "تم حذف العميل بنجاح", "warning");
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
