// واجهة خدمة إرسال رمز التحقق عبر واتساب - Standing Tech Gateway
using CraftsmanAccounts.Application.Common;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IOtpSenderService
{
    Task<ServiceResult> SendAsync(string phoneNumber, string otpCode);
}
