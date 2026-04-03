// متحكم المشاريع - إدارة المشاريع وتعيين العمال مع إشعارات OneSignal
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class ProjectsController : BaseApiController
{
    private readonly IProjectService _service;
    private readonly IUserNotificationService _notify;
    public ProjectsController(IProjectService service, IUserNotificationService notify)
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
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        var result = await _service.CreateAsync(GetUserId(), request);
        if (result.Success)
        {
            await _notify.NotifyUserAsync(GetUserId(), "إضافة مشروع", $"تم إضافة المشروع {request.Name} بنجاح", "success");
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
        return BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectRequest request)
    {
        var result = await _service.UpdateAsync(GetUserId(), id, request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "تحديث مشروع", "تم تحديث بيانات المشروع بنجاح", "info");
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(GetUserId(), id);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "حذف مشروع", "تم حذف المشروع بنجاح", "warning");
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{projectId}/workers")]
    public async Task<IActionResult> GetWorkers(int projectId)
    {
        var result = await _service.GetProjectWorkersAsync(GetUserId(), projectId);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpPost("{projectId}/workers")]
    public async Task<IActionResult> AssignWorkers(int projectId, [FromBody] AssignWorkersRequest request)
    {
        var result = await _service.AssignWorkersAsync(GetUserId(), projectId, request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "تعيين عمال", "تم تعيين العمال للمشروع بنجاح", "success");
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // نقاط نهاية إسناد المعدات للمشاريع
    [HttpGet("{projectId}/equipment")]
    public async Task<IActionResult> GetEquipment(int projectId)
    {
        var result = await _service.GetProjectEquipmentAsync(GetUserId(), projectId);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpPost("{projectId}/equipment")]
    public async Task<IActionResult> AssignEquipment(int projectId, [FromBody] AssignEquipmentRequest request)
    {
        var result = await _service.AssignEquipmentAsync(GetUserId(), projectId, request);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "إسناد معدات", "تم إسناد المعدات للمشروع بنجاح", "success");
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{projectId}/release-equipment")]
    public async Task<IActionResult> ReleaseEquipment(int projectId)
    {
        var result = await _service.ReleaseEquipmentAsync(GetUserId(), projectId);
        if (result.Success)
            await _notify.NotifyUserAsync(GetUserId(), "تحرير معدات", "تم تحرير جميع معدات المشروع", "info");
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
