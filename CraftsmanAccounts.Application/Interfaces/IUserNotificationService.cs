// واجهة خدمة الإشعارات - تعريف عمليات إرسال الإشعارات للمستخدمين عبر OneSignal
namespace CraftsmanAccounts.Application.Interfaces;

/// <summary>
/// واجهة خدمة الإشعارات لإرسال إشعارات للمستخدمين عبر OneSignal
/// </summary>
public interface IUserNotificationService
{
    /// <summary>
    /// إرسال إشعار لمستخدم محدد عبر معرف المستخدم الخارجي
    /// </summary>
    Task NotifyUserAsync(int userId, string title, string message, string type);

    /// <summary>
    /// إرسال إشعار لجميع المستخدمين المشتركين
    /// </summary>
    Task NotifyAllAsync(string title, string message, string type);
}
