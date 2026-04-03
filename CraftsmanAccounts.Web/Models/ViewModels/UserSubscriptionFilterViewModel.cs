// نموذج فلتر اشتراكات المستخدمين - بحث وتصفية وترقيم صفحات الاشتراكات
namespace CraftsmanAccounts.Web.Models.ViewModels;

public class UserSubscriptionFilterViewModel
{
    public string? SearchTerm { get; set; }
    public SubscriptionStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public List<UserSubscription> Subscriptions { get; set; } = new();
}
