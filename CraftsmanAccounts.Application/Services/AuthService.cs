// خدمة المصادقة - تسجيل الدخول والتسجيل وإرسال رمز التحقق وإعادة تعيين كلمة المرور
using System.Security.Cryptography;
using System.Text;
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IOtpSenderService _otpSender;

    public AuthService(IUnitOfWork uow, IOtpSenderService otpSender)
    {
        _uow = uow;
        _otpSender = otpSender;
    }

    // الحد الأقصى لمحاولات إرسال OTP قبل القفل
    private const int MaxOtpAttempts = 5;
    // مدة القفل بالدقائق عند تجاوز الحد
    private const int LockoutMinutes = 30;

    public async Task<ServiceResult> SendOtpAsync(string phoneNumber)
    {
        var user = (await _uow.Repository<AppUser>().FindAsync(u => u.PhoneNumber == phoneNumber)).FirstOrDefault();

        // التحقق من القفل المؤقت
        if (user != null && user.OtpLockedUntil.HasValue && user.OtpLockedUntil > DateTime.UtcNow)
        {
            var remaining = (int)(user.OtpLockedUntil.Value - DateTime.UtcNow).TotalMinutes + 1;
            return ServiceResult.Fail($"تم تجاوز الحد المسموح. يرجى المحاولة بعد {remaining} دقيقة");
        }

        var otp = Random.Shared.Next(1000, 9999).ToString();

        if (user == null)
        {
            user = new AppUser { PhoneNumber = phoneNumber, OtpCode = otp, OtpExpiry = DateTime.UtcNow.AddMinutes(5), OtpAttemptCount = 1 };
            await _uow.Repository<AppUser>().AddAsync(user);
        }
        else
        {
            // إعادة تعيين العداد إذا انتهت فترة القفل
            if (user.OtpLockedUntil.HasValue && user.OtpLockedUntil <= DateTime.UtcNow)
            {
                user.OtpAttemptCount = 0;
                user.OtpLockedUntil = null;
            }

            user.OtpAttemptCount++;

            // قفل الحساب عند تجاوز الحد
            if (user.OtpAttemptCount > MaxOtpAttempts)
            {
                user.OtpLockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                _uow.Repository<AppUser>().Update(user);
                await _uow.SaveChangesAsync();
                return ServiceResult.Fail($"تم تجاوز الحد المسموح لإرسال رمز التحقق. يرجى المحاولة بعد {LockoutMinutes} دقيقة");
            }

            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            _uow.Repository<AppUser>().Update(user);
        }
        await _uow.SaveChangesAsync();

        // إرسال رمز التحقق عبر واتساب
        var sendResult = await _otpSender.SendAsync(phoneNumber, otp);
        if (!sendResult.Success)
            return ServiceResult.Fail(sendResult.Message);

        return ServiceResult.Ok("تم إرسال رمز التحقق بنجاح");
    }

    public async Task<ServiceResult> VerifyOtpAsync(string phoneNumber, string otpCode)
    {
        var user = (await _uow.Repository<AppUser>().FindAsync(u => u.PhoneNumber == phoneNumber)).FirstOrDefault();
        if (user == null) return ServiceResult.Fail("المستخدم غير موجود");
        if (user.OtpCode != otpCode || user.OtpExpiry < DateTime.UtcNow)
            return ServiceResult.Fail("رمز التحقق غير صحيح أو منتهي الصلاحية");

        user.IsPhoneVerified = true;
        user.OtpCode = null;
        user.OtpExpiry = null;
        user.OtpAttemptCount = 0;
        user.OtpLockedUntil = null;
        _uow.Repository<AppUser>().Update(user);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم التحقق من الرقم بنجاح");
    }

    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existing = (await _uow.Repository<AppUser>().FindAsync(u => u.PhoneNumber == request.PhoneNumber)).FirstOrDefault();
        if (existing != null && !string.IsNullOrEmpty(existing.PasswordHash))
            return ServiceResult<AuthResponse>.Fail("رقم الهاتف مسجل مسبقاً");

        if (existing != null && !existing.IsPhoneVerified)
            return ServiceResult<AuthResponse>.Fail("يجب التحقق من رقم الهاتف أولاً");

        var user = existing ?? new AppUser { PhoneNumber = request.PhoneNumber };
        user.FullName = request.FullName;
        user.Address = request.Address;
        user.PasswordHash = HashPassword(request.Password);
        user.ApprovalStatus = ApprovalStatus.Pending;

        if (existing == null)
            await _uow.Repository<AppUser>().AddAsync(user);
        else
            _uow.Repository<AppUser>().Update(user);

        await _uow.SaveChangesAsync();
        return ServiceResult<AuthResponse>.Ok(new AuthResponse(user.Id, user.FullName, ""), "تم التسجيل بنجاح، بانتظار الموافقة");
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = (await _uow.Repository<AppUser>().FindAsync(u => u.PhoneNumber == request.PhoneNumber)).FirstOrDefault();
        if (user == null || user.PasswordHash != HashPassword(request.Password))
            return ServiceResult<AuthResponse>.Fail("بيانات الدخول غير صحيحة");
        if (user.ApprovalStatus != ApprovalStatus.Approved)
            return ServiceResult<AuthResponse>.Fail("الحساب غير مفعل بعد");
        if (!user.IsActive)
            return ServiceResult<AuthResponse>.Fail("الحساب معطل");

        return ServiceResult<AuthResponse>.Ok(new AuthResponse(user.Id, user.FullName, ""));
    }

    public async Task<ServiceResult> ForgotPasswordAsync(string phoneNumber)
    {
        return await SendOtpAsync(phoneNumber);
    }

    public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = (await _uow.Repository<AppUser>().FindAsync(u => u.PhoneNumber == request.PhoneNumber)).FirstOrDefault();
        if (user == null) return ServiceResult.Fail("المستخدم غير موجود");
        if (user.OtpCode != request.OtpCode || user.OtpExpiry < DateTime.UtcNow)
            return ServiceResult.Fail("رمز التحقق غير صحيح أو منتهي الصلاحية");

        user.PasswordHash = HashPassword(request.NewPassword);
        user.OtpCode = null;
        user.OtpExpiry = null;
        user.OtpAttemptCount = 0;
        user.OtpLockedUntil = null;
        _uow.Repository<AppUser>().Update(user);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم تغيير كلمة المرور بنجاح");
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    // تحديث التوكن باستخدام رمز التحديث
    public async Task<ServiceResult<AuthResponse>> RefreshTokenAsync(int userId, string refreshToken)
    {
        var user = await _uow.Repository<AppUser>().Query().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return ServiceResult<AuthResponse>.Fail("المستخدم غير موجود");
        if (user.RefreshToken != refreshToken) return ServiceResult<AuthResponse>.Fail("رمز التحديث غير صالح");
        if (user.RefreshTokenExpiry < DateTime.UtcNow) return ServiceResult<AuthResponse>.Fail("رمز التحديث منتهي الصلاحية");

        // التوكن الجديد سيُولّد في الكونترولر - هنا نعيد البيانات فقط
        return ServiceResult<AuthResponse>.Ok(new AuthResponse(user.Id, user.FullName, "", ""));
    }

    // إبطال رمز التحديث (تسجيل الخروج)
    public async Task<ServiceResult> RevokeRefreshTokenAsync(int userId)
    {
        var user = await _uow.Repository<AppUser>().Query().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return ServiceResult.Fail("المستخدم غير موجود");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        _uow.Repository<AppUser>().Update(user);
        await _uow.SaveChangesAsync();
        return ServiceResult.Ok("تم تسجيل الخروج بنجاح");
    }
}
