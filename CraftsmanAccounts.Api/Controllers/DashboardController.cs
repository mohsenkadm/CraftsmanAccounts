// متحكم لوحة التحكم - إحصائيات المستخدم والتحليلات البيانية
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class DashboardController : BaseApiController
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service) => _service = service;

    // إحصائيات المستخدم الشاملة (أرقام وملخصات)
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var result = await _service.GetStatisticsAsync(GetUserId());
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    // تحليل البيانات للمخططات البيانية والجداول
    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics([FromQuery] int? year)
    {
        var result = await _service.GetAnalyticsAsync(GetUserId(), year);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }
}
