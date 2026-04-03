// واجهة خدمة إدارة اشتراكات المستخدمين - الموافقة والرفض على طلبات الاشتراك
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IUserSubscriptionManagementService
{
    Task<ServiceResult<PagedResult<UserSubscriptionDto>>> GetAllAsync(PagedRequest request);
    Task<ServiceResult> ApproveAsync(int id);
    Task<ServiceResult> RejectAsync(int id);
}
