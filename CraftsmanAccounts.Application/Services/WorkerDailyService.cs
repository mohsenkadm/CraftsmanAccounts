// خدمة اليوميات - سجل يومي لأجور العمال وإمكانية دفع دفعات متعددة
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class WorkerDailyService : IWorkerDailyService
{
    private readonly IUnitOfWork _uow;
    public WorkerDailyService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<PagedResult<WorkerDailyDto>>> GetAllAsync(int userId, int? projectId, AccountStatementRequest request)
    {
        var q = _uow.Repository<WorkerDaily>().Query()
            .Where(wd => wd.UserId == userId)
            .Include(wd => wd.Project)
            .Include(wd => wd.Entries).ThenInclude(e => e.Worker)
            .AsQueryable();

        if (projectId.HasValue) q = q.Where(wd => wd.ProjectId == projectId.Value);
        if (request.FromDate.HasValue) q = q.Where(wd => wd.Date >= request.FromDate.Value);
        if (request.ToDate.HasValue) q = q.Where(wd => wd.Date <= request.ToDate.Value);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(wd => wd.Date)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .ToListAsync();

        var dtos = items.Select(wd => new WorkerDailyDto(
            wd.Id, wd.ProjectId, wd.Project.Name, wd.Date, wd.IsPaid,
            wd.Entries.Select(e => new WorkerDailyEntryDto(e.Id, e.WorkerId, e.Worker.Name, e.DailyRate, e.ExtraAmount)).ToList()
        )).ToList();

        return ServiceResult<PagedResult<WorkerDailyDto>>.Ok(new PagedResult<WorkerDailyDto> { Items = dtos, TotalCount = total, Page = request.Page, PageSize = request.PageSize });
    }

    public async Task<ServiceResult<WorkerDailyDto>> CreateAsync(int userId, CreateWorkerDailyRequest request)
    {
        var project = await _uow.Repository<Project>().Query()
            .Include(p => p.ProjectWorkers)
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.UserId == userId);
        if (project == null) return ServiceResult<WorkerDailyDto>.Fail("المشروع غير موجود");

        var workerDaily = new WorkerDaily { UserId = userId, ProjectId = request.ProjectId, Date = request.Date, WalletId = request.WalletId };

        foreach (var entry in request.Entries)
        {
            var pw = project.ProjectWorkers.FirstOrDefault(pw => pw.WorkerId == entry.WorkerId);
            if (pw == null) continue;

            workerDaily.Entries.Add(new WorkerDailyEntry
            {
                WorkerId = entry.WorkerId,
                DailyRate = pw.DailyRate,
                ExtraAmount = entry.ExtraAmount
            });
        }

        await _uow.Repository<WorkerDaily>().AddAsync(workerDaily);

        // إنشاء قيود محاسبية لكل عامل
        foreach (var entry in workerDaily.Entries)
        {
            var totalAmount = entry.DailyRate + entry.ExtraAmount;
            await _uow.Repository<AccountEntry>().AddAsync(new AccountEntry
            {
                UserId = userId,
                EntryType = AccountEntryType.Debit,
                Category = AccountEntryCategory.WorkerSalary,
                Amount = totalAmount,
                Description = $"يومية عامل - مشروع: {project.Name}",
                WorkerId = entry.WorkerId,
                ProjectId = request.ProjectId,
                WalletId = request.WalletId
            });
        }

        await _uow.SaveChangesAsync();

        var dto = new WorkerDailyDto(workerDaily.Id, workerDaily.ProjectId, project.Name, workerDaily.Date, workerDaily.IsPaid,
            workerDaily.Entries.Select(e => new WorkerDailyEntryDto(e.Id, e.WorkerId, "", e.DailyRate, e.ExtraAmount)).ToList());
        return ServiceResult<WorkerDailyDto>.Ok(dto);
    }

    public async Task<ServiceResult> PayBatchAsync(int userId, List<int> workerDailyIds, int walletId)
    {
        var wallet = await _uow.Repository<Wallet>().Query().FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId);
        if (wallet == null) return ServiceResult.Fail("المحفظة غير موجودة");

        var dailies = await _uow.Repository<WorkerDaily>().Query()
            .Where(wd => workerDailyIds.Contains(wd.Id) && wd.UserId == userId && !wd.IsPaid)
            .Include(wd => wd.Entries)
            .ToListAsync();

        decimal totalAmount = 0;
        foreach (var daily in dailies)
        {
            daily.IsPaid = true;
            daily.WalletId = walletId;
            _uow.Repository<WorkerDaily>().Update(daily);
            totalAmount += daily.Entries.Sum(e => e.DailyRate + e.ExtraAmount);
        }

        wallet.Balance -= totalAmount;
        _uow.Repository<Wallet>().Update(wallet);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok($"تم دفع {dailies.Count} يومية بمبلغ {totalAmount}");
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        var wd = await _uow.Repository<WorkerDaily>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (wd == null) return ServiceResult.Fail("اليومية غير موجودة");
        _uow.Repository<WorkerDaily>().Remove(wd);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
