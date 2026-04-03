// نتيجة العملية مع بيانات - تُعيد حالة النجاح أو الفشل مع بيانات إضافية
namespace CraftsmanAccounts.Application.Common;

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Ok(T data, string message = "") => new() { Success = true, Data = data, Message = message };
    public new static ServiceResult<T> Fail(string message) => new() { Success = false, Message = message };
}
