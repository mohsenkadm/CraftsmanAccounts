// واجهة خدمة المصادقة - تسجيل الدخول والتسجيل وإعادة تعيين كلمة المرور
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> SendOtpAsync(string phoneNumber);
    Task<ServiceResult> VerifyOtpAsync(string phoneNumber, string otpCode);
    Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ServiceResult> ForgotPasswordAsync(string phoneNumber);
    Task<ServiceResult> ResetPasswordAsync(ResetPasswordRequest request);
    Task<ServiceResult<AuthResponse>> RefreshTokenAsync(int userId, string refreshToken);
    Task<ServiceResult> RevokeRefreshTokenAsync(int userId);
}
