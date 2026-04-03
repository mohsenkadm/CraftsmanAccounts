// حالة الاشتراك
namespace CraftsmanAccounts.Domain.Enums;

public enum SubscriptionStatus
{
    Pending,   // بانتظار الموافقة
    Approved,  // مقبول
    Rejected,  // مرفوض
    Paid       // مدفوع
}
