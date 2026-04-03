// خدمة المعدات - إضافة معدات وتسجيل التلف مع قيود محاسبية وتتبع الكمية المتاحة
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IUnitOfWork _uow;
    public EquipmentService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<List<EquipmentDto>>> GetAllAsync(int userId)
    {
        var items = await _uow.Repository<Equipment>().Query()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new EquipmentDto(
                e.Id, e.Name, e.PurchasedFrom, e.Amount, e.Quantity,
                e.Quantity - e.ProjectEquipments.Where(pe => pe.Project.IsActive).Sum(pe => pe.Quantity),
                e.IsDamaged, e.CreatedAt))
            .ToListAsync();
        return ServiceResult<List<EquipmentDto>>.Ok(items);
    }

    public async Task<ServiceResult<List<EquipmentDto>>> GetAvailableAsync(int userId)
    {
        var items = await _uow.Repository<Equipment>().Query()
            .Where(e => e.UserId == userId && !e.IsDamaged)
            .Select(e => new { e, Available = e.Quantity - e.ProjectEquipments.Where(pe => pe.Project.IsActive).Sum(pe => pe.Quantity) })
            .Where(x => x.Available > 0)
            .OrderByDescending(x => x.e.CreatedAt)
            .Select(x => new EquipmentDto(
                x.e.Id, x.e.Name, x.e.PurchasedFrom, x.e.Amount, x.e.Quantity,
                x.Available, x.e.IsDamaged, x.e.CreatedAt))
            .ToListAsync();
        return ServiceResult<List<EquipmentDto>>.Ok(items);
    }

    public async Task<ServiceResult<EquipmentDto>> CreateAsync(int userId, CreateEquipmentRequest request)
    {
        var equipment = new Equipment { UserId = userId, Name = request.Name, PurchasedFrom = request.PurchasedFrom, Amount = request.Amount, Quantity = request.Quantity };
        await _uow.Repository<Equipment>().AddAsync(equipment);
        await _uow.SaveChangesAsync();
        return ServiceResult<EquipmentDto>.Ok(new EquipmentDto(equipment.Id, equipment.Name, equipment.PurchasedFrom, equipment.Amount, equipment.Quantity, equipment.Quantity, false, equipment.CreatedAt));
    }

    public async Task<ServiceResult> MarkDamagedAsync(int userId, int id)
    {
        var e = await _uow.Repository<Equipment>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (e == null) return ServiceResult.Fail("المعدة غير موجودة");
        if (e.IsDamaged) return ServiceResult.Fail("المعدة مسجلة كتالفة مسبقاً");

        e.IsDamaged = true;
        _uow.Repository<Equipment>().Update(e);

        // تسجيل خسارة المعدة في القيود المحاسبية
        await _uow.Repository<AccountEntry>().AddAsync(new AccountEntry
        {
            UserId = userId, EntryType = AccountEntryType.Debit, Category = AccountEntryCategory.EquipmentLoss,
            Amount = e.Amount, Description = $"تلف معدة: {e.Name}"
        });

        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم تسجيل التلف");
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        var e = await _uow.Repository<Equipment>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (e == null) return ServiceResult.Fail("المعدة غير موجودة");
        _uow.Repository<Equipment>().Remove(e);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    // كشف المعدات الشامل (كمي ومالي) مع فلاتر التاريخ والحالة والمشروع
    public async Task<ServiceResult<EquipmentStatementDto>> GetStatementAsync(int userId, EquipmentStatementRequest request)
    {
        var query = _uow.Repository<Equipment>().Query()
            .Where(e => e.UserId == userId);

        // فلتر التاريخ
        if (request.FromDate.HasValue)
            query = query.Where(e => e.CreatedAt >= request.FromDate.Value);
        if (request.ToDate.HasValue)
            query = query.Where(e => e.CreatedAt <= request.ToDate.Value);

        // فلتر حالة التلف
        if (request.IsDamaged.HasValue)
            query = query.Where(e => e.IsDamaged == request.IsDamaged.Value);

        // فلتر المشروع
        if (request.ProjectId.HasValue)
            query = query.Where(e => e.ProjectEquipments.Any(pe => pe.ProjectId == request.ProjectId.Value));

        // فلتر البحث
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(e => e.Name.Contains(request.SearchTerm) || e.PurchasedFrom.Contains(request.SearchTerm));

        // حساب الملخص قبل التقسيم
        var summaryData = await query.Select(e => new
        {
            e.Quantity,
            e.Amount,
            e.IsDamaged,
            Assigned = e.ProjectEquipments.Where(pe => pe.Project.IsActive).Sum(pe => pe.Quantity)
        }).ToListAsync();

        var totalEquipmentCount = summaryData.Count;
        var totalQuantity = summaryData.Sum(s => s.Quantity);
        var assignedQuantity = summaryData.Sum(s => s.Assigned);
        var availableQuantity = summaryData.Sum(s => s.Quantity - s.Assigned);
        var damagedCount = summaryData.Count(s => s.IsDamaged);
        var totalValue = summaryData.Sum(s => s.Amount * s.Quantity);
        var totalDamagedValue = summaryData.Where(s => s.IsDamaged).Sum(s => s.Amount * s.Quantity);

        // ترتيب
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortDescending ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
            "amount" => request.SortDescending ? query.OrderByDescending(e => e.Amount) : query.OrderBy(e => e.Amount),
            "quantity" => request.SortDescending ? query.OrderByDescending(e => e.Quantity) : query.OrderBy(e => e.Quantity),
            _ => query.OrderByDescending(e => e.CreatedAt)
        };

        // تقسيم الصفحات
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EquipmentStatementItemDto(
                e.Id, e.Name, e.PurchasedFrom, e.Amount, e.Quantity,
                e.Quantity - e.ProjectEquipments.Where(pe => pe.Project.IsActive).Sum(pe => pe.Quantity),
                e.ProjectEquipments.Where(pe => pe.Project.IsActive).Sum(pe => pe.Quantity),
                e.IsDamaged, e.CreatedAt))
            .ToListAsync();

        var paged = new PagedResult<EquipmentStatementItemDto>
        {
            Items = items,
            TotalCount = totalEquipmentCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var statement = new EquipmentStatementDto(
            totalEquipmentCount, totalQuantity, availableQuantity, assignedQuantity,
            damagedCount, totalValue, totalDamagedValue, paged);

        return ServiceResult<EquipmentStatementDto>.Ok(statement);
    }
}
