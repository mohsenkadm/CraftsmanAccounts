// كائنات نقل بيانات العملاء - إضافة وتعديل وعرض العملاء
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات العميل
public record ClientDto(int Id, string Name, string PhoneNumber, string Address, ClientType ClientType, bool IsActive);

// طلب إنشاء عميل جديد
public record CreateClientRequest(string Name, string PhoneNumber, string Address, ClientType ClientType);

// طلب تحديث بيانات عميل
public record UpdateClientRequest(string Name, string PhoneNumber, string Address, ClientType ClientType, bool IsActive);
