// خدمة مصادقة المدير - تسجيل دخول لوحة التحكم
using System.Security.Cryptography;
using System.Text;
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;

namespace CraftsmanAccounts.Application.Services;

public class AdminAuthService : IAdminAuthService
{
    private readonly IUnitOfWork _uow;
    public AdminAuthService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<AdminAuthResponse>> LoginAsync(AdminLoginRequest request)
    {
        var admin = (await _uow.Repository<Admin>().FindAsync(a => a.Username == request.Username)).FirstOrDefault();
        if (admin == null) return ServiceResult<AdminAuthResponse>.Fail("بيانات الدخول غير صحيحة");

        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(request.Password)));
        if (admin.PasswordHash != hash)
            return ServiceResult<AdminAuthResponse>.Fail("بيانات الدخول غير صحيحة");

        return ServiceResult<AdminAuthResponse>.Ok(new AdminAuthResponse(admin.Id, admin.DisplayName, ""));
    }
}
