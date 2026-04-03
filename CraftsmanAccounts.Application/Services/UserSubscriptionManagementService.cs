// خدمة إدارة اشتراكات المستخدمين - الموافقة والرفض على طلبات الاشتراك
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class UserSubscriptionManagementService : IUserSubscriptionManagementService
{
    private readonly IUnitOfWork _uow;
    public UserSubscriptionManagementService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<PagedResult<UserSubscriptionDto>>> GetAllAsync(PagedRequest request)
    {
        var q = _uow.Repository<UserSubscription>().Query()
            .Include(us => us.User)
            .Include(us => us.SubscriptionType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            q = q.Where(us => us.User.FullName.Contains(request.SearchTerm));

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(us => us.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(us => new UserSubscriptionDto(us.Id, us.UserId, us.User.FullName, us.SubscriptionTypeId, us.SubscriptionType.Name, us.Amount, us.StartDate, us.EndDate, us.Status, us.IsPaid))
            .ToListAsync();

        return ServiceResult<PagedResult<UserSubscriptionDto>>.Ok(new PagedResult<UserSubscriptionDto> { Items = items, TotalCount = total, Page = request.Page, PageSize = request.PageSize });
    }

    public async Task<ServiceResult> ApproveAsync(int id)
    {
        var us = await _uow.Repository<UserSubscription>().GetByIdAsync(id);
        if (us == null) return ServiceResult.Fail("الاشتراك غير موجود");
        us.Status = SubscriptionStatus.Approved;
        _uow.Repository<UserSubscription>().Update(us);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تمت الموافقة على الاشتراك");
    }

    public async Task<ServiceResult> RejectAsync(int id)
    {
        var us = await _uow.Repository<UserSubscription>().GetByIdAsync(id);
        if (us == null) return ServiceResult.Fail("الاشتراك غير موجود");
        us.Status = SubscriptionStatus.Rejected;
        _uow.Repository<UserSubscription>().Update(us);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم رفض الاشتراك");
    }
}
