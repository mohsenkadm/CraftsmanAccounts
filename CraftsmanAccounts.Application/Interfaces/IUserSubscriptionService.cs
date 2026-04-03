// واجهة خدمة اشتراكات المستخدم - جلب اشتراكات المستخدم والاشتراك بنوع جديد
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IUserSubscriptionService
{
    Task<ServiceResult<List<UserSubscriptionDto>>> GetMySubscriptionsAsync(int userId);
    Task<ServiceResult<UserSubscriptionDto>> GetCurrentSubscriptionAsync(int userId);
    Task<ServiceResult<UserSubscriptionDto>> SubscribeAsync(int userId, CreateUserSubscriptionRequest request);
}
