// متحكم المحافظ المالية - إنشاء وعرض وحذف المحافظ مع إشعارات OneSignal
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class WalletsController : BaseApiController
{
    private readonly IWalletService _service;
    private readonly IUserNotificationService _notify;
    public WalletsController(IWalletService service, IUserNotificationService notify)
    {
        _service = service;
        _notify = notify;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok((await _service.GetAllAsync(GetUserId())).Data);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWalletRequest request)
    {
        var result = await _service.CreateAsync(GetUserId(), request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "إضافة محفظة", $"تم إضافة المحفظة {request.Name} بنجاح", "success");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "حذف محفظة", "تم حذف المحفظة بنجاح", "warning");
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
