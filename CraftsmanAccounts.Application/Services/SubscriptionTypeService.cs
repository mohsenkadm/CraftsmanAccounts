// خدمة أنواع الاشتراكات - إدارة أنواع الاشتراكات المتاحة
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class SubscriptionTypeService : ISubscriptionTypeService
{
    private readonly IUnitOfWork _uow;
    public SubscriptionTypeService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<List<SubscriptionTypeDto>>> GetAllActiveAsync()
    {
        var items = await _uow.Repository<SubscriptionType>().Query()
            .Where(s => s.IsActive)
            .Select(s => new SubscriptionTypeDto(s.Id, s.Name, s.Amount, s.DurationInDays, s.Details, s.IsActive))
            .ToListAsync();
        return ServiceResult<List<SubscriptionTypeDto>>.Ok(items);
    }

    public async Task<ServiceResult<List<SubscriptionTypeDto>>> GetAllAsync()
    {
        var items = await _uow.Repository<SubscriptionType>().Query()
            .Select(s => new SubscriptionTypeDto(s.Id, s.Name, s.Amount, s.DurationInDays, s.Details, s.IsActive))
            .ToListAsync();
        return ServiceResult<List<SubscriptionTypeDto>>.Ok(items);
    }

    public async Task<ServiceResult<SubscriptionTypeDto>> CreateAsync(CreateSubscriptionTypeRequest request)
    {
        var st = new SubscriptionType { Name = request.Name, Amount = request.Amount, DurationInDays = request.DurationInDays, Details = request.Details, IsActive = request.IsActive };
        await _uow.Repository<SubscriptionType>().AddAsync(st);
        await _uow.SaveChangesAsync();
        return ServiceResult<SubscriptionTypeDto>.Ok(new SubscriptionTypeDto(st.Id, st.Name, st.Amount, st.DurationInDays, st.Details, st.IsActive));
    }

    public async Task<ServiceResult<SubscriptionTypeDto>> UpdateAsync(int id, UpdateSubscriptionTypeRequest request)
    {
        var st = await _uow.Repository<SubscriptionType>().GetByIdAsync(id);
        if (st == null) return ServiceResult<SubscriptionTypeDto>.Fail("نوع الاشتراك غير موجود");
        st.Name = request.Name; st.Amount = request.Amount; st.DurationInDays = request.DurationInDays; st.Details = request.Details; st.IsActive = request.IsActive;
        _uow.Repository<SubscriptionType>().Update(st);
        await _uow.SaveChangesAsync();
        return ServiceResult<SubscriptionTypeDto>.Ok(new SubscriptionTypeDto(st.Id, st.Name, st.Amount, st.DurationInDays, st.Details, st.IsActive));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var st = await _uow.Repository<SubscriptionType>().GetByIdAsync(id);
        if (st == null) return ServiceResult.Fail("نوع الاشتراك غير موجود");
        _uow.Repository<SubscriptionType>().Remove(st);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}
