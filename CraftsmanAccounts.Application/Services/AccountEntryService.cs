// خدمة القيود المحاسبية - كشوفات العمال والعملاء والمشاريع وتقرير الأرباح والخسائر
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class AccountEntryService : IAccountEntryService
{
    private readonly IUnitOfWork _uow;
    public AccountEntryService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<AccountStatementDto>> GetWorkerStatementAsync(int userId, int workerId, AccountStatementRequest request)
    {
        return await GetStatementAsync(userId, request, q => q.Where(a => a.WorkerId == workerId));
    }

    public async Task<ServiceResult<AccountStatementDto>> GetClientStatementAsync(int userId, int clientId, AccountStatementRequest request)
    {
        return await GetStatementAsync(userId, request, q => q.Where(a => a.ClientId == clientId));
    }

    public async Task<ServiceResult<AccountStatementDto>> GetProjectStatementAsync(int userId, int projectId, AccountStatementRequest request)
    {
        return await GetStatementAsync(userId, request, q => q.Where(a => a.ProjectId == projectId));
    }

    // كشف حساب عام بدون فلتر كيان محدد
    public async Task<ServiceResult<AccountStatementDto>> GetAllStatementAsync(int userId, AccountStatementRequest request)
    {
        return await GetStatementAsync(userId, request, q => q);
    }

    // عملية مشتركة لجلب كشف حساب مع فلترة وتقسيم صفحات وإجماليات
    private async Task<ServiceResult<AccountStatementDto>> GetStatementAsync(int userId, AccountStatementRequest request, Func<IQueryable<AccountEntry>, IQueryable<AccountEntry>> filter)
    {
        IQueryable<AccountEntry> q = _uow.Repository<AccountEntry>().Query()
            .Where(a => a.UserId == userId)
            .Include(a => a.Worker).Include(a => a.Client).Include(a => a.Project);

        q = filter(q);

        if (request.FromDate.HasValue) q = q.Where(a => a.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue) q = q.Where(a => a.CreatedAt <= request.ToDate.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            q = q.Where(a => a.Description.Contains(request.SearchTerm));
        if (!string.IsNullOrWhiteSpace(request.EntryType) && Enum.TryParse<AccountEntryType>(request.EntryType, true, out var entryType))
            q = q.Where(a => a.EntryType == entryType);
        if (!string.IsNullOrWhiteSpace(request.Category) && Enum.TryParse<AccountEntryCategory>(request.Category, true, out var category))
            q = q.Where(a => a.Category == category);

        // حساب الإجماليات قبل التقسيم
        var totalCredits = await q.Where(a => a.EntryType == AccountEntryType.Credit).SumAsync(a => a.Amount);
        var totalDebits = await q.Where(a => a.EntryType == AccountEntryType.Debit).SumAsync(a => a.Amount);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(a => new AccountEntryDto(a.Id, a.EntryType.ToString(), a.Category.ToString(), a.Amount, a.Description, a.CreatedAt,
                a.WorkerId, a.Worker != null ? a.Worker.Name : null,
                a.ClientId, a.Client != null ? a.Client.Name : null,
                a.ProjectId, a.Project != null ? a.Project.Name : null))
            .ToListAsync();

        var paged = new PagedResult<AccountEntryDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize };
        return ServiceResult<AccountStatementDto>.Ok(new AccountStatementDto(totalCredits, totalDebits, totalCredits - totalDebits, paged));
    }

    public async Task<ServiceResult<ProfitLossDto>> GetProfitLossAsync(int userId, DateTime? fromDate, DateTime? toDate)
    {
        var q = _uow.Repository<AccountEntry>().Query().Where(a => a.UserId == userId);
        if (fromDate.HasValue) q = q.Where(a => a.CreatedAt >= fromDate.Value);
        if (toDate.HasValue) q = q.Where(a => a.CreatedAt <= toDate.Value);

        var entries = await q.Include(a => a.Worker).Include(a => a.Client).Include(a => a.Project).OrderByDescending(a => a.CreatedAt).ToListAsync();

        var totalReceipts = entries.Where(e => e.EntryType == AccountEntryType.Credit && e.Category == AccountEntryCategory.Receipt).Sum(e => e.Amount);
        var totalPayments = entries.Where(e => e.EntryType == AccountEntryType.Debit && e.Category != AccountEntryCategory.EquipmentLoss).Sum(e => e.Amount);
        var equipmentLosses = entries.Where(e => e.Category == AccountEntryCategory.EquipmentLoss).Sum(e => e.Amount);

        var dtos = entries.Select(a => new AccountEntryDto(a.Id, a.EntryType.ToString(), a.Category.ToString(), a.Amount, a.Description, a.CreatedAt,
            a.WorkerId, a.Worker?.Name, a.ClientId, a.Client?.Name, a.ProjectId, a.Project?.Name)).ToList();

        return ServiceResult<ProfitLossDto>.Ok(new ProfitLossDto(totalReceipts, totalPayments, equipmentLosses, totalReceipts - totalPayments - equipmentLosses, dtos));
    }

    // كشف الحركات المالية اليومية
    public async Task<ServiceResult<DailyMovementsDto>> GetDailyMovementsAsync(int userId, DateTime? date)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;
        var startOfDay = targetDate.Date;
        var endOfDay = startOfDay.AddDays(1);

        var entries = await _uow.Repository<AccountEntry>().Query()
            .Where(a => a.UserId == userId && a.CreatedAt >= startOfDay && a.CreatedAt < endOfDay)
            .Include(a => a.Worker).Include(a => a.Client).Include(a => a.Project)
            .OrderByDescending(a => a.CreatedAt).ToListAsync();

        var totalCredits = entries.Where(e => e.EntryType == AccountEntryType.Credit).Sum(e => e.Amount);
        var totalDebits = entries.Where(e => e.EntryType == AccountEntryType.Debit).Sum(e => e.Amount);

        var dtos = entries.Select(a => new AccountEntryDto(a.Id, a.EntryType.ToString(), a.Category.ToString(), a.Amount, a.Description, a.CreatedAt,
            a.WorkerId, a.Worker?.Name, a.ClientId, a.Client?.Name, a.ProjectId, a.Project?.Name)).ToList();

        return ServiceResult<DailyMovementsDto>.Ok(new DailyMovementsDto(startOfDay, totalCredits, totalDebits, totalCredits - totalDebits, dtos));
    }
}
