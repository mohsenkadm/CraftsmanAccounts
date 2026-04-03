// خدمة أنواع الاشتراكات الوهمية - بيانات تجريبية للاشتراكات للاختبار
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Services.Mock;

public class MockSubscriptionTypeService : ISubscriptionTypeService
{
    private static readonly List<SubscriptionType> Subscriptions =
    [
        new() { Id = 1, Name = "الاشتراك الشهري", Amount = 25000, DurationInDays = 30, Details = "اشتراك شهري يتضمن جميع الميزات الأساسية مع دعم فني على مدار الساعة", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-6) },
        new() { Id = 2, Name = "الاشتراك ربع السنوي", Amount = 60000, DurationInDays = 90, Details = "اشتراك لمدة ثلاثة أشهر بسعر مخفض مع ميزات إضافية وأولوية في الدعم الفني", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-5) },
        new() { Id = 3, Name = "الاشتراك نصف السنوي", Amount = 100000, DurationInDays = 180, Details = "اشتراك لمدة ستة أشهر مع خصم كبير وجميع الميزات المتقدمة والدعم المباشر", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-4) },
        new() { Id = 4, Name = "الاشتراك السنوي", Amount = 180000, DurationInDays = 365, Details = "اشتراك سنوي بأفضل سعر يتضمن جميع الميزات المتقدمة والدعم الحصري وتحديثات مجانية", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-3) }
    ];

    private static int _nextId = 5;

    public List<SubscriptionType> GetAll() => Subscriptions.OrderByDescending(s => s.CreatedAt).ToList();

    public SubscriptionType? GetById(int id) => Subscriptions.FirstOrDefault(s => s.Id == id);

    public void Create(SubscriptionType subscriptionType)
    {
        subscriptionType.Id = _nextId++;
        subscriptionType.CreatedAt = DateTime.Now;
        Subscriptions.Add(subscriptionType);
    }

    public void Update(SubscriptionType subscriptionType)
    {
        var existing = Subscriptions.FirstOrDefault(s => s.Id == subscriptionType.Id);
        if (existing is null) return;

        existing.Name = subscriptionType.Name;
        existing.Amount = subscriptionType.Amount;
        existing.DurationInDays = subscriptionType.DurationInDays;
        existing.Details = subscriptionType.Details;
        existing.IsActive = subscriptionType.IsActive;
    }

    public void Delete(int id) => Subscriptions.RemoveAll(s => s.Id == id);
}
