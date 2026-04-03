// خدمة سندات القبض - إنشاء سندات قبض عامة ولمشاريع مع تحديث المحافظ والقيود المحاسبية
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class ReceiptService : IReceiptService
{
    private readonly IUnitOfWork _uow;
    public ReceiptService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<PagedResult<ReceiptDto>>> GetAllAsync(int userId, AccountStatementRequest request)
    {
        var q = _uow.Repository<Receipt>().Query()
            .Where(r => r.UserId == userId)
            .Include(r => r.Client).Include(r => r.Project).Include(r => r.Wallet).AsQueryable();

        if (request.FromDate.HasValue) q = q.Where(r => r.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue) q = q.Where(r => r.CreatedAt <= request.ToDate.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            q = q.Where(r => r.Details.Contains(request.SearchTerm));

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(r => r.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(r => new ReceiptDto(r.Id, r.ReceiptType.ToString(), r.Amount, r.Details, r.ClientId, r.Client != null ? r.Client.Name : null, r.ProjectId, r.Project != null ? r.Project.Name : null, r.WalletId, r.Wallet != null ? r.Wallet.Name : null, r.CreatedAt))
            .ToListAsync();

        return ServiceResult<PagedResult<ReceiptDto>>.Ok(new PagedResult<ReceiptDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize });
    }

    public async Task<ServiceResult<ReceiptDto>> CreateGeneralAsync(int userId, CreateReceiptGeneralRequest request)
    {
        var receipt = new Receipt { UserId = userId, ReceiptType = ReceiptType.General, Amount = request.Amount, Details = request.Details, ClientId = request.ClientId, WalletId = request.WalletId };
        await _uow.Repository<Receipt>().AddAsync(receipt);

        // إنشاء قيد محاسبي
        await _uow.Repository<AccountEntry>().AddAsync(new AccountEntry
        {
            UserId = userId, EntryType = AccountEntryType.Credit, Category = AccountEntryCategory.Receipt,
            Amount = request.Amount, Description = request.Details, ClientId = request.ClientId, WalletId = request.WalletId
        });

        // تحديث رصيد المحفظة
        if (request.WalletId.HasValue)
        {
            var wallet = await _uow.Repository<Wallet>().GetByIdAsync(request.WalletId.Value);
            if (wallet != null) { wallet.Balance += request.Amount; _uow.Repository<Wallet>().Update(wallet); }
        }

        await _uow.SaveChangesAsync();
        return ServiceResult<ReceiptDto>.Ok(new ReceiptDto(receipt.Id, "General", receipt.Amount, receipt.Details, receipt.ClientId, null, null, null, receipt.WalletId, null, receipt.CreatedAt));
    }

    public async Task<ServiceResult<ReceiptDto>> CreateProjectAsync(int userId, CreateReceiptProjectRequest request)
    {
        var project = await _uow.Repository<Project>().Query().Include(p => p.Client).FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.UserId == userId);
        if (project == null) return ServiceResult<ReceiptDto>.Fail("المشروع غير موجود");

        var receipt = new Receipt { UserId = userId, ReceiptType = ReceiptType.Project, Amount = request.Amount, Details = request.Details, ProjectId = request.ProjectId, ClientId = project.ClientId, WalletId = request.WalletId };
        await _uow.Repository<Receipt>().AddAsync(receipt);

        // تحديث المبلغ المدفوع للمشروع
        project.PaidAmount += request.Amount;
        _uow.Repository<Project>().Update(project);

        // إنشاء قيود محاسبية
        await _uow.Repository<AccountEntry>().AddAsync(new AccountEntry
        {
            UserId = userId, EntryType = AccountEntryType.Credit, Category = AccountEntryCategory.Receipt,
            Amount = request.Amount, Description = request.Details, ProjectId = request.ProjectId, ClientId = project.ClientId, WalletId = request.WalletId
        });
        await _uow.Repository<AccountEntry>().AddAsync(new AccountEntry
        {
            UserId = userId, EntryType = AccountEntryType.Credit, Category = AccountEntryCategory.ClientPayment,
            Amount = request.Amount, Description = $"سند قبض مشروع: {project.Name}", ClientId = project.ClientId, ProjectId = request.ProjectId
        });

        if (request.WalletId.HasValue)
        {
            var wallet = await _uow.Repository<Wallet>().GetByIdAsync(request.WalletId.Value);
            if (wallet != null) { wallet.Balance += request.Amount; _uow.Repository<Wallet>().Update(wallet); }
        }

        await _uow.SaveChangesAsync();
        return ServiceResult<ReceiptDto>.Ok(new ReceiptDto(receipt.Id, "Project", receipt.Amount, receipt.Details, project.ClientId, project.Client.Name, project.Id, project.Name, receipt.WalletId, null, receipt.CreatedAt));
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        var r = await _uow.Repository<Receipt>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (r == null) return ServiceResult.Fail("السند غير موجود");
        _uow.Repository<Receipt>().Remove(r);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
