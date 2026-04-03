// متحكم أنواع المصروفات - إضافة وعرض وحذف تصنيفات المصاريف مع إشعارات OneSignal
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class ExpenseTypesController : BaseApiController
{
    private readonly IExpenseTypeService _service;
    private readonly IUserNotificationService _notify;
    public ExpenseTypesController(IExpenseTypeService service, IUserNotificationService notify)
    {
        _service = service;
        _notify = notify;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok((await _service.GetAllAsync(GetUserId())).Data);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseTypeRequest request)
    {
        var result = await _service.CreateAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "إضافة نوع مصروف", $"تم إضافة نوع المصروف {request.Name} بنجاح", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "حذف نوع مصروف", "تم حذف نوع المصروف بنجاح", "warning");
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
