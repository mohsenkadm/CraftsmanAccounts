// كيان اشتراك المستخدم - يربط المستخدم بنوع اشتراك معين
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Domain.Entities;

public class UserSubscription : BaseEntity
{
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public int SubscriptionTypeId { get; set; }
    public SubscriptionType SubscriptionType { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;
    public bool IsPaid { get; set; }
}
