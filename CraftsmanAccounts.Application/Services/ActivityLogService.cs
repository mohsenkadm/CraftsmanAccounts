// خدمة سجل النشاطات - تسجيل العمليات وجلب السجل
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IUnitOfWork _uow;
    public ActivityLogService(IUnitOfWork uow) => _uow = uow;

    public async Task LogAsync(int userId, string action, string entityName, int? entityId, string? details, string? ipAddress)
    {
        await _uow.Repository<ActivityLog>().AddAsync(new ActivityLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details,
            IpAddress = ipAddress
        });
        await _uow.SaveChangesAsync();
    }

    public async Task<ServiceResult<PagedResult<ActivityLogDto>>> GetLogsAsync(int userId, ActivityLogRequest request)
    {
        var q = _uow.Repository<ActivityLog>().Query().Where(a => a.UserId == userId);

        if (request.FromDate.HasValue) q = q.Where(a => a.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue) q = q.Where(a => a.CreatedAt <= request.ToDate.Value);
        if (!string.IsNullOrWhiteSpace(request.EntityName)) q = q.Where(a => a.EntityName == request.EntityName);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm)) q = q.Where(a => a.Details != null && a.Details.Contains(request.SearchTerm));

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(a => new ActivityLogDto(a.Id, a.Action, a.EntityName, a.EntityId, a.Details, a.IpAddress, a.CreatedAt))
            .ToListAsync();

        return ServiceResult<PagedResult<ActivityLogDto>>.Ok(new PagedResult<ActivityLogDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize });
    }
}
