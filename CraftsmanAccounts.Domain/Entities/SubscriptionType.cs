// كيان نوع الاشتراك - يحدد أنواع الاشتراكات المتاحة ومدتها وأسعارها
namespace CraftsmanAccounts.Domain.Entities;

public class SubscriptionType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int DurationInDays { get; set; }
    public string Details { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
