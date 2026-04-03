// متحكم الكشوفات المحاسبية - كشوفات العمال والعملاء والمشاريع وتقرير الأرباح والخسائر
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraftsmanAccounts.Api.Controllers;

[Authorize]
public class StatementsController : BaseApiController
{
    private readonly IAccountEntryService _service;
    public StatementsController(IAccountEntryService service) => _service = service;

    // كشف حساب عامل معين
    [HttpGet("worker/{workerId}")]
    public async Task<IActionResult> WorkerStatement(int workerId, [FromQuery] AccountStatementRequest request)
    {
        var result = await _service.GetWorkerStatementAsync(GetUserId(), workerId, request);
        return Ok(result.Data);
    }

    // كشف حساب عميل معين
    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> ClientStatement(int clientId, [FromQuery] AccountStatementRequest request)
    {
        var result = await _service.GetClientStatementAsync(GetUserId(), clientId, request);
        return Ok(result.Data);
    }

    // كشف حساب مشروع معين
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> ProjectStatement(int projectId, [FromQuery] AccountStatementRequest request)
    {
        var result = await _service.GetProjectStatementAsync(GetUserId(), projectId, request);
        return Ok(result.Data);
    }

    // كشف حساب عام لكل القيود
    [HttpGet("all")]
    public async Task<IActionResult> AllStatement([FromQuery] AccountStatementRequest request)
    {
        var result = await _service.GetAllStatementAsync(GetUserId(), request);
        return Ok(result.Data);
    }

    [HttpGet("profit-loss")]
    public async Task<IActionResult> ProfitLoss([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var result = await _service.GetProfitLossAsync(GetUserId(), fromDate, toDate);
        return Ok(result.Data);
    }

    // كشف الحركات المالية اليومية
    [HttpGet("daily-movements")]
    public async Task<IActionResult> DailyMovements([FromQuery] DateTime? date)
    {
        var result = await _service.GetDailyMovementsAsync(GetUserId(), date);
        return Ok(result.Data);
    }
}
