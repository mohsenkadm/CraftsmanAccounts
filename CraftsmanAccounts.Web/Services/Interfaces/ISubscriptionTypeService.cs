// واجهة خدمة أنواع الاشتراكات - تعريف عمليات CRUD لأنواع الاشتراكات
using CraftsmanAccounts.Web.Models;

namespace CraftsmanAccounts.Web.Services.Interfaces;

/// <summary>
/// واجهة خدمة أنواع الاشتراكات للإنشاء والتحديث والحذف والاستعلام
/// </summary>
public interface ISubscriptionTypeService
{
    List<SubscriptionType> GetAll();
    SubscriptionType? GetById(int id);
    void Create(SubscriptionType subscriptionType);
    void Update(SubscriptionType subscriptionType);
    void Delete(int id);
}
