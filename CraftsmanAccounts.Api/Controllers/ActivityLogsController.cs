// متحكم سجل النشاطات - عرض سجل عمليات المستخدم
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class ActivityLogsController : BaseApiController
{
    private readonly IActivityLogService _service;
    public ActivityLogsController(IActivityLogService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] ActivityLogRequest request)
    {
        var result = await _service.GetLogsAsync(GetUserId(), request);
        return Ok(result.Data);
    }
}
