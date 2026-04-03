// واجهة خدمة أنواع الاشتراكات - إدارة أنواع الاشتراكات من قبل المدير
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface ISubscriptionTypeService
{
    Task<ServiceResult<List<SubscriptionTypeDto>>> GetAllActiveAsync();
    Task<ServiceResult<List<SubscriptionTypeDto>>> GetAllAsync();
    Task<ServiceResult<SubscriptionTypeDto>> CreateAsync(CreateSubscriptionTypeRequest request);
    Task<ServiceResult<SubscriptionTypeDto>> UpdateAsync(int id, UpdateSubscriptionTypeRequest request);
    Task<ServiceResult> DeleteAsync(int id);
}
