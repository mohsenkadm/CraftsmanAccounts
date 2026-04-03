// واجهة خدمة اشتراكات المستخدمين - تعريف عمليات إدارة الاشتراكات والموافقات والمدفوعات
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Models.ViewModels;

namespace CraftsmanAccounts.Web.Services.Interfaces;

/// <summary>
/// واجهة خدمة اشتراكات المستخدمين للموافقة والرفض وتسجيل الدفع والإحصائيات
/// </summary>
public interface IUserSubscriptionService
{
    UserSubscriptionFilterViewModel GetSubscriptions(UserSubscriptionFilterViewModel filter);
    UserSubscription? GetById(int id);
    void Approve(int id);
    void Reject(int id);
    void MarkAsPaid(int id);
    int GetTotalCount();
    int GetActiveCount();
    decimal GetTotalRevenue();
    List<UserSubscription> GetRecent(int count);
}
