// واجهة خدمة إدارة المستخدمين - الموافقة والرفض وتفعيل/تعطيل الحسابات
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IUserManagementService
{
    Task<ServiceResult<PagedResult<AppUserDto>>> GetAllUsersAsync(PagedRequest request, ApprovalStatus? statusFilter = null);
    Task<ServiceResult<AppUserDto>> GetUserByIdAsync(int id);
    Task<ServiceResult> ApproveUserAsync(int id);
    Task<ServiceResult> RejectUserAsync(int id);
    Task<ServiceResult> ToggleUserActiveAsync(int id);
    Task<int> GetPendingCountAsync();
}
