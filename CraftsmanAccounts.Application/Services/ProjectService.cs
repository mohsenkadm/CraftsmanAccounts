// خدمة المشاريع - إدارة المشاريع وتعيين العمال وإسناد المعدات وجلب التفاصيل الكاملة
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _uow;
    public ProjectService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<List<ProjectDto>>> GetAllAsync(int userId)
    {
        var projects = await _uow.Repository<Project>().Query()
            .Where(p => p.UserId == userId)
            .Include(p => p.Client)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto(p.Id, p.Name, p.TotalAmount, p.PaidAmount, p.Address, p.StartDate, p.EndDate, p.IsActive, p.ClientId, p.Client.Name, p.Client.ClientType))
            .ToListAsync();
        return ServiceResult<List<ProjectDto>>.Ok(projects);
    }

    public async Task<ServiceResult<ProjectDetailDto>> GetByIdAsync(int userId, int id)
    {
        var p = await _uow.Repository<Project>().Query()
            .Include(x => x.Client)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (p == null) return ServiceResult<ProjectDetailDto>.Fail("المشروع غير موجود");

        // جلب العمال المعينين
        var workers = await _uow.Repository<ProjectWorker>().Query()
            .Where(pw => pw.ProjectId == id).Include(pw => pw.Worker)
            .Select(pw => new ProjectWorkerDto(pw.Id, pw.WorkerId, pw.Worker.Name, pw.DailyRate)).ToListAsync();

        // جلب المعدات المسندة
        var equipments = await _uow.Repository<ProjectEquipment>().Query()
            .Where(pe => pe.ProjectId == id).Include(pe => pe.Equipment)
            .Select(pe => new ProjectEquipmentDto(pe.Id, pe.EquipmentId, pe.Equipment.Name, pe.Quantity)).ToListAsync();

        // جلب سندات الصرف
        var payments = await _uow.Repository<Payment>().Query()
            .Where(pay => pay.ProjectId == id && pay.UserId == userId)
            .Include(pay => pay.ExpenseType).Include(pay => pay.Client).Include(pay => pay.Worker).Include(pay => pay.Wallet)
            .OrderByDescending(pay => pay.CreatedAt)
            .Select(pay => new PaymentDto(pay.Id, pay.PaymentType.ToString(), pay.Amount, pay.Details,
                pay.ExpenseTypeId, pay.ExpenseType != null ? pay.ExpenseType.Name : null,
                pay.ProjectId, p.Name, pay.ClientId, pay.Client != null ? pay.Client.Name : null,
                pay.WorkerId, pay.Worker != null ? pay.Worker.Name : null,
                pay.WalletId, pay.Wallet != null ? pay.Wallet.Name : null, pay.CreatedAt)).ToListAsync();

        // جلب سندات القبض
        var receipts = await _uow.Repository<Receipt>().Query()
            .Where(r => r.ProjectId == id && r.UserId == userId)
            .Include(r => r.Client).Include(r => r.Wallet)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReceiptDto(r.Id, r.ReceiptType.ToString(), r.Amount, r.Details,
                r.ClientId, r.Client != null ? r.Client.Name : null,
                r.ProjectId, p.Name, r.WalletId, r.Wallet != null ? r.Wallet.Name : null, r.CreatedAt)).ToListAsync();

        // جلب اليوميات
        var dailies = await _uow.Repository<WorkerDaily>().Query()
            .Where(wd => wd.ProjectId == id && wd.UserId == userId)
            .Include(wd => wd.Entries).ThenInclude(e => e.Worker)
            .OrderByDescending(wd => wd.Date)
            .Select(wd => new WorkerDailyDto(wd.Id, wd.ProjectId, p.Name, wd.Date, wd.IsPaid,
                wd.Entries.Select(e => new WorkerDailyEntryDto(e.Id, e.WorkerId, e.Worker.Name, e.DailyRate, e.ExtraAmount)).ToList())).ToListAsync();

        // جلب القيود المحاسبية
        var entries = await _uow.Repository<AccountEntry>().Query()
            .Where(ae => ae.ProjectId == id && ae.UserId == userId)
            .Include(ae => ae.Worker).Include(ae => ae.Client)
            .OrderByDescending(ae => ae.CreatedAt)
            .Select(ae => new AccountEntryDto(ae.Id, ae.EntryType.ToString(), ae.Category.ToString(), ae.Amount, ae.Description, ae.CreatedAt,
                ae.WorkerId, ae.Worker != null ? ae.Worker.Name : null,
                ae.ClientId, ae.Client != null ? ae.Client.Name : null,
                ae.ProjectId, p.Name)).ToListAsync();

        return ServiceResult<ProjectDetailDto>.Ok(new ProjectDetailDto(
            p.Id, p.Name, p.TotalAmount, p.PaidAmount, p.Address, p.StartDate, p.EndDate, p.IsActive,
            p.ClientId, p.Client.Name, p.Client.ClientType,
            workers, equipments, payments, receipts, dailies, entries));
    }

    public async Task<ServiceResult<ProjectDto>> CreateAsync(int userId, CreateProjectRequest request)
    {
        var client = await _uow.Repository<Client>().Query().FirstOrDefaultAsync(c => c.Id == request.ClientId && c.UserId == userId);
        if (client == null) return ServiceResult<ProjectDto>.Fail("العميل غير موجود");

        var project = new Project { UserId = userId, Name = request.Name, TotalAmount = request.TotalAmount, Address = request.Address, ClientId = request.ClientId, StartDate = request.StartDate, EndDate = request.EndDate };
        await _uow.Repository<Project>().AddAsync(project);
        await _uow.SaveChangesAsync();
        return ServiceResult<ProjectDto>.Ok(new ProjectDto(project.Id, project.Name, project.TotalAmount, 0, project.Address, project.StartDate, project.EndDate, project.IsActive, project.ClientId, client.Name, client.ClientType));
    }

    public async Task<ServiceResult<ProjectDto>> UpdateAsync(int userId, int id, UpdateProjectRequest request)
    {
        var p = await _uow.Repository<Project>().Query().Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (p == null) return ServiceResult<ProjectDto>.Fail("المشروع غير موجود");
        p.Name = request.Name; p.TotalAmount = request.TotalAmount; p.Address = request.Address; p.ClientId = request.ClientId; p.StartDate = request.StartDate; p.EndDate = request.EndDate; p.IsActive = request.IsActive;
        _uow.Repository<Project>().Update(p);
        await _uow.SaveChangesAsync();

        var client = await _uow.Repository<Client>().GetByIdAsync(p.ClientId);
        return ServiceResult<ProjectDto>.Ok(new ProjectDto(p.Id, p.Name, p.TotalAmount, p.PaidAmount, p.Address, p.StartDate, p.EndDate, p.IsActive, p.ClientId, client!.Name, client.ClientType));
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        var p = await _uow.Repository<Project>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (p == null) return ServiceResult.Fail("المشروع غير موجود");
        _uow.Repository<Project>().Remove(p);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult<List<ProjectWorkerDto>>> GetProjectWorkersAsync(int userId, int projectId)
    {
        var project = await _uow.Repository<Project>().Query().FirstOrDefaultAsync(x => x.Id == projectId && x.UserId == userId);
        if (project == null) return ServiceResult<List<ProjectWorkerDto>>.Fail("المشروع غير موجود");

        var workers = await _uow.Repository<ProjectWorker>().Query()
            .Where(pw => pw.ProjectId == projectId)
            .Include(pw => pw.Worker)
            .Select(pw => new ProjectWorkerDto(pw.Id, pw.WorkerId, pw.Worker.Name, pw.DailyRate))
            .ToListAsync();
        return ServiceResult<List<ProjectWorkerDto>>.Ok(workers);
    }

    public async Task<ServiceResult> AssignWorkersAsync(int userId, int projectId, AssignWorkersRequest request)
    {
        var project = await _uow.Repository<Project>().Query().FirstOrDefaultAsync(x => x.Id == projectId && x.UserId == userId);
        if (project == null) return ServiceResult.Fail("المشروع غير موجود");

        var existing = await _uow.Repository<ProjectWorker>().FindAsync(pw => pw.ProjectId == projectId);
        foreach (var e in existing) _uow.Repository<ProjectWorker>().Remove(e);

        foreach (var item in request.Workers)
            await _uow.Repository<ProjectWorker>().AddAsync(new ProjectWorker { ProjectId = projectId, WorkerId = item.WorkerId, DailyRate = item.DailyRate });

        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم تعيين العمال بنجاح");
    }

    public async Task<ServiceResult<List<ProjectEquipmentDto>>> GetProjectEquipmentAsync(int userId, int projectId)
    {
        var project = await _uow.Repository<Project>().Query().FirstOrDefaultAsync(x => x.Id == projectId && x.UserId == userId);
        if (project == null) return ServiceResult<List<ProjectEquipmentDto>>.Fail("المشروع غير موجود");

        var items = await _uow.Repository<ProjectEquipment>().Query()
            .Where(pe => pe.ProjectId == projectId)
            .Include(pe => pe.Equipment)
            .Select(pe => new ProjectEquipmentDto(pe.Id, pe.EquipmentId, pe.Equipment.Name, pe.Quantity))
            .ToListAsync();
        return ServiceResult<List<ProjectEquipmentDto>>.Ok(items);
    }

    public async Task<ServiceResult> AssignEquipmentAsync(int userId, int projectId, AssignEquipmentRequest request)
    {
        var project = await _uow.Repository<Project>().Query().FirstOrDefaultAsync(x => x.Id == projectId && x.UserId == userId);
        if (project == null) return ServiceResult.Fail("المشروع غير موجود");

        // التحقق من توفر الكميات المطلوبة
        foreach (var item in request.Equipments)
        {
            var eq = await _uow.Repository<Equipment>().Query()
                .Where(e => e.Id == item.EquipmentId && e.UserId == userId && !e.IsDamaged)
                .Select(e => new { e.Quantity, Used = e.ProjectEquipments.Where(pe => pe.Project.IsActive && pe.ProjectId != projectId).Sum(pe => pe.Quantity) })
                .FirstOrDefaultAsync();
            if (eq == null) return ServiceResult.Fail($"المعدة رقم {item.EquipmentId} غير موجودة أو تالفة");
            if (eq.Quantity - eq.Used < item.Quantity) return ServiceResult.Fail($"الكمية المتاحة للمعدة رقم {item.EquipmentId} غير كافية");
        }

        // حذف التعيينات القديمة
        var existing = await _uow.Repository<ProjectEquipment>().FindAsync(pe => pe.ProjectId == projectId);
        foreach (var e in existing) _uow.Repository<ProjectEquipment>().Remove(e);

        // إضافة التعيينات الجديدة
        foreach (var item in request.Equipments)
            await _uow.Repository<ProjectEquipment>().AddAsync(new ProjectEquipment { ProjectId = projectId, EquipmentId = item.EquipmentId, Quantity = item.Quantity });

        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم إسناد المعدات للمشروع بنجاح");
    }

    public async Task<ServiceResult> ReleaseEquipmentAsync(int userId, int projectId)
    {
        var project = await _uow.Repository<Project>().Query().FirstOrDefaultAsync(x => x.Id == projectId && x.UserId == userId);
        if (project == null) return ServiceResult.Fail("المشروع غير موجود");

        var existing = await _uow.Repository<ProjectEquipment>().FindAsync(pe => pe.ProjectId == projectId);
        foreach (var e in existing) _uow.Repository<ProjectEquipment>().Remove(e);

        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم تفريغ جميع معدات المشروع");
    }
}
