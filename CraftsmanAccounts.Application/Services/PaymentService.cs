// خدمة سندات الصرف - إنشاء مدفوعات متنوعة مع تحديث المحافظ والقيود المحاسبية
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;
    public PaymentService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<PagedResult<PaymentDto>>> GetAllAsync(int userId, AccountStatementRequest request)
    {
        var q = _uow.Repository<Payment>().Query()
            .Where(p => p.UserId == userId)
            .Include(p => p.ExpenseType).Include(p => p.Project).Include(p => p.Client).Include(p => p.Worker).Include(p => p.Wallet).AsQueryable();

        if (request.FromDate.HasValue) q = q.Where(p => p.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue) q = q.Where(p => p.CreatedAt <= request.ToDate.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            q = q.Where(p => p.Details.Contains(request.SearchTerm));

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(p => new PaymentDto(p.Id, p.PaymentType.ToString(), p.Amount, p.Details,
                p.ExpenseTypeId, p.ExpenseType != null ? p.ExpenseType.Name : null,
                p.ProjectId, p.Project != null ? p.Project.Name : null,
                p.ClientId, p.Client != null ? p.Client.Name : null,
                p.WorkerId, p.Worker != null ? p.Worker.Name : null,
                p.WalletId, p.Wallet != null ? p.Wallet.Name : null, p.CreatedAt))
            .ToListAsync();

        return ServiceResult<PagedResult<PaymentDto>>.Ok(new PagedResult<PaymentDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize });
    }

    // عملية مشتركة لإنشاء سند صرف مع القيد المحاسبي وتحديث المحفظة
    private async Task CreatePaymentCommon(int userId, PaymentType type, decimal amount, string details, int? expenseTypeId, int? projectId, int? clientId, int? workerId, int? walletId, AccountEntryCategory category)
    {
        var payment = new Payment { UserId = userId, PaymentType = type, Amount = amount, Details = details, ExpenseTypeId = expenseTypeId, ProjectId = projectId, ClientId = clientId, WorkerId = workerId, WalletId = walletId };
        await _uow.Repository<Payment>().AddAsync(payment);

        await _uow.Repository<AccountEntry>().AddAsync(new AccountEntry
        {
            UserId = userId, EntryType = AccountEntryType.Debit, Category = category,
            Amount = amount, Description = details, WorkerId = workerId, ClientId = clientId, ProjectId = projectId, WalletId = walletId
        });

        if (walletId.HasValue)
        {
            var wallet = await _uow.Repository<Wallet>().GetByIdAsync(walletId.Value);
            if (wallet != null) { wallet.Balance -= amount; _uow.Repository<Wallet>().Update(wallet); }
        }
    }

    public async Task<ServiceResult<PaymentDto>> CreateGeneralAsync(int userId, CreatePaymentGeneralRequest request)
    {
        await CreatePaymentCommon(userId, PaymentType.General, request.Amount, request.Details, request.ExpenseTypeId, null, null, null, request.WalletId, AccountEntryCategory.GeneralExpense);
        await _uow.SaveChangesAsync();
        return ServiceResult<PaymentDto>.Ok(new PaymentDto(0, "General", request.Amount, request.Details, request.ExpenseTypeId, null, null, null, null, null, null, null, request.WalletId, null, DateTime.UtcNow));
    }

    public async Task<ServiceResult<PaymentDto>> CreateProjectAsync(int userId, CreatePaymentProjectRequest request)
    {
        var project = await _uow.Repository<Project>().Query().FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.UserId == userId);
        if (project == null) return ServiceResult<PaymentDto>.Fail("المشروع غير موجود");

        await CreatePaymentCommon(userId, PaymentType.Project, request.Amount, request.Details, null, request.ProjectId, null, null, request.WalletId, AccountEntryCategory.ProjectExpense);
        await _uow.SaveChangesAsync();
        return ServiceResult<PaymentDto>.Ok(new PaymentDto(0, "Project", request.Amount, request.Details, null, null, request.ProjectId, project.Name, null, null, null, null, request.WalletId, null, DateTime.UtcNow));
    }

    public async Task<ServiceResult<PaymentDto>> CreateClientAsync(int userId, CreatePaymentClientRequest request)
    {
        var client = await _uow.Repository<Client>().Query().FirstOrDefaultAsync(c => c.Id == request.ClientId && c.UserId == userId);
        if (client == null) return ServiceResult<PaymentDto>.Fail("العميل غير موجود");

        await CreatePaymentCommon(userId, PaymentType.Client, request.Amount, request.Details, null, null, request.ClientId, null, request.WalletId, AccountEntryCategory.ClientPayment);
        await _uow.SaveChangesAsync();
        return ServiceResult<PaymentDto>.Ok(new PaymentDto(0, "Client", request.Amount, request.Details, null, null, null, null, request.ClientId, client.Name, null, null, request.WalletId, null, DateTime.UtcNow));
    }

    public async Task<ServiceResult<PaymentDto>> CreateWorkerAsync(int userId, CreatePaymentWorkerRequest request)
    {
        var worker = await _uow.Repository<Worker>().Query().FirstOrDefaultAsync(w => w.Id == request.WorkerId && w.UserId == userId);
        if (worker == null) return ServiceResult<PaymentDto>.Fail("العامل غير موجود");

        await CreatePaymentCommon(userId, PaymentType.Worker, request.Amount, request.Details, null, null, null, request.WorkerId, request.WalletId, AccountEntryCategory.WorkerPayment);
        await _uow.SaveChangesAsync();
        return ServiceResult<PaymentDto>.Ok(new PaymentDto(0, "Worker", request.Amount, request.Details, null, null, null, null, null, null, request.WorkerId, worker.Name, request.WalletId, null, DateTime.UtcNow));
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        var p = await _uow.Repository<Payment>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (p == null) return ServiceResult.Fail("السند غير موجود");
        _uow.Repository<Payment>().Remove(p);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
