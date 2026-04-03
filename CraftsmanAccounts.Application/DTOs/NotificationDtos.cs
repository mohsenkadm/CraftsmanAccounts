// كائنات نقل بيانات الإشعارات
namespace CraftsmanAccounts.Application.DTOs;

// عرض إشعار
public record NotificationDto(int Id, string Title, string Message, string Type, bool IsRead, DateTime CreatedAt);
