// واجهة خدمة مصادقة المدير - تسجيل دخول لوحة الإدارة
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IAdminAuthService
{
    Task<ServiceResult<AdminAuthResponse>> LoginAsync(AdminLoginRequest request);
}
