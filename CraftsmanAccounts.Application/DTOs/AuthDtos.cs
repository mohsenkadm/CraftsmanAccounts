// كائنات نقل بيانات المصادقة - تسجيل الدخول والتسجيل وإعادة تعيين كلمة المرور
namespace CraftsmanAccounts.Application.DTOs;

// طلب إرسال رمز التحقق
public record SendOtpRequest(string PhoneNumber);

// طلب التحقق من رمز OTP
public record VerifyOtpRequest(string PhoneNumber, string OtpCode);

// طلب تسجيل مستخدم جديد
public record RegisterRequest(string FullName, string Address, string PhoneNumber, string Password);

// طلب تسجيل الدخول
public record LoginRequest(string PhoneNumber, string Password);

// طلب نسيان كلمة المرور
public record ForgotPasswordRequest(string PhoneNumber);

// طلب إعادة تعيين كلمة المرور
public record ResetPasswordRequest(string PhoneNumber, string OtpCode, string NewPassword);

// استجابة المصادقة للمستخدم
public record AuthResponse(int UserId, string FullName, string Token, string RefreshToken = "");

// طلب تحديث التوكن
public record RefreshTokenRequest(string Token, string RefreshToken);

// طلب تسجيل دخول المدير
public record AdminLoginRequest(string Username, string Password);

// استجابة مصادقة المدير
public record AdminAuthResponse(int AdminId, string DisplayName, string Token);
