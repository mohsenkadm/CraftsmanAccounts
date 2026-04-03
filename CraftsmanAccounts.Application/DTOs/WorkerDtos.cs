// كائنات نقل بيانات العمال - إضافة وتعديل وعرض العمال
namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات العامل
public record WorkerDto(int Id, string Name, string Address, string PhoneNumber, bool IsActive);

// طلب إنشاء عامل جديد
public record CreateWorkerRequest(string Name, string Address, string PhoneNumber);

// طلب تحديث بيانات عامل
public record UpdateWorkerRequest(string Name, string Address, string PhoneNumber, bool IsActive);
