// واجهة خدمة الإشعارات - جلب إشعارات المستخدم وتحديث حالة القراءة
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface INotificationService
{
    Task<ServiceResult<PagedResult<NotificationDto>>> GetByUserAsync(int userId, PagedRequest request);
    Task<ServiceResult> MarkAsReadAsync(int userId, int notificationId);
    Task<ServiceResult> MarkAllAsReadAsync(int userId);
    Task<ServiceResult<int>> GetUnreadCountAsync(int userId);
}
