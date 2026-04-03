// خدمة اشتراكات المستخدم - جلب اشتراكات المستخدم والاشتراك بنوع جديد
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class UserSubscriptionService : IUserSubscriptionService
{
    private readonly IUnitOfWork _uow;
    public UserSubscriptionService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<List<UserSubscriptionDto>>> GetMySubscriptionsAsync(int userId)
    {
        var items = await _uow.Repository<UserSubscription>().Query()
            .Where(us => us.UserId == userId)
            .Include(us => us.User)
            .Include(us => us.SubscriptionType)
            .OrderByDescending(us => us.CreatedAt)
            .Select(us => new UserSubscriptionDto(us.Id, us.UserId, us.User.FullName, us.SubscriptionTypeId,
                us.SubscriptionType.Name, us.Amount, us.StartDate, us.EndDate, us.Status, us.IsPaid))
            .ToListAsync();

        return ServiceResult<List<UserSubscriptionDto>>.Ok(items);
    }

    public async Task<ServiceResult<UserSubscriptionDto>> GetCurrentSubscriptionAsync(int userId)
    {
        var us = await _uow.Repository<UserSubscription>().Query()
            .Where(s => s.UserId == userId && (s.Status == SubscriptionStatus.Approved || s.Status == SubscriptionStatus.Paid) && s.EndDate >= DateTime.UtcNow)
            .Include(s => s.User)
            .Include(s => s.SubscriptionType)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync();

        if (us == null) return ServiceResult<UserSubscriptionDto>.Fail("لا يوجد اشتراك فعّال");

        return ServiceResult<UserSubscriptionDto>.Ok(new UserSubscriptionDto(us.Id, us.UserId, us.User.FullName,
            us.SubscriptionTypeId, us.SubscriptionType.Name, us.Amount, us.StartDate, us.EndDate, us.Status, us.IsPaid));
    }

    public async Task<ServiceResult<UserSubscriptionDto>> SubscribeAsync(int userId, CreateUserSubscriptionRequest request)
    {
        var subType = await _uow.Repository<SubscriptionType>().GetByIdAsync(request.SubscriptionTypeId);
        if (subType == null || !subType.IsActive)
            return ServiceResult<UserSubscriptionDto>.Fail("نوع الاشتراك غير موجود أو غير متاح");

        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(subType.DurationInDays);

        var subscription = new UserSubscription
        {
            UserId = userId,
            SubscriptionTypeId = subType.Id,
            Amount = subType.Amount,
            StartDate = startDate,
            EndDate = endDate,
            Status = SubscriptionStatus.Pending,
            IsPaid = false
        };

        await _uow.Repository<UserSubscription>().AddAsync(subscription);
        await _uow.SaveChangesAsync();

        var user = await _uow.Repository<AppUser>().GetByIdAsync(userId);
        return ServiceResult<UserSubscriptionDto>.Ok(new UserSubscriptionDto(subscription.Id, userId,
            user?.FullName ?? "", subType.Id, subType.Name, subscription.Amount, startDate, endDate, subscription.Status, false));
    }
}
