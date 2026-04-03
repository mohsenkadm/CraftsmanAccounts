// نموذج عرض لوحة التحكم - إحصائيات المستخدمين والاشتراكات والإيرادات
namespace CraftsmanAccounts.Web.Models.ViewModels;

public class DashboardViewModel
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int PendingApprovals { get; set; }
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<AppUser> RecentUsers { get; set; } = new();
    public List<UserSubscription> RecentSubscriptions { get; set; } = new();
}
