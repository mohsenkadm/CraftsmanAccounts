// نموذج اشتراك المستخدم - بيانات اشتراك المستخدم وحالته
namespace CraftsmanAccounts.Web.Models;

public class UserSubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int SubscriptionTypeId { get; set; }
    public string SubscriptionTypeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;
    public bool IsPaid { get; set; }
}

public enum SubscriptionStatus
{
    Pending,
    Approved,
    Rejected,
    Paid
}
