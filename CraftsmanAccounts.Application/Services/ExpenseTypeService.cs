// خدمة أنواع المصروفات - إضافة وعرض وحذف تصنيفات المصاريف
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class ExpenseTypeService : IExpenseTypeService
{
    private readonly IUnitOfWork _uow;
    public ExpenseTypeService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<List<ExpenseTypeDto>>> GetAllAsync(int userId)
    {
        var types = await _uow.Repository<ExpenseType>().Query()
            .Where(e => e.UserId == userId)
            .Select(e => new ExpenseTypeDto(e.Id, e.Name))
            .ToListAsync();
        return ServiceResult<List<ExpenseTypeDto>>.Ok(types);
    }

    public async Task<ServiceResult<ExpenseTypeDto>> CreateAsync(int userId, CreateExpenseTypeRequest request)
    {
        var et = new ExpenseType { UserId = userId, Name = request.Name };
        await _uow.Repository<ExpenseType>().AddAsync(et);
        await _uow.SaveChangesAsync();
        return ServiceResult<ExpenseTypeDto>.Ok(new ExpenseTypeDto(et.Id, et.Name));
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        var et = await _uow.Repository<ExpenseType>().Query().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (et == null) return ServiceResult.Fail("نوع المصروف غير موجود");
        _uow.Repository<ExpenseType>().Remove(et);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
