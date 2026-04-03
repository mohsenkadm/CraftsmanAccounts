// نتيجة العملية - تُعيد حالة النجاح أو الفشل مع رسالة
namespace CraftsmanAccounts.Application.Common;

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ServiceResult Ok(string message = "") => new() { Success = true, Message = message };
    public static ServiceResult Fail(string message) => new() { Success = false, Message = message };
}
