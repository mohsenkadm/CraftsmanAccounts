// نموذج نوع الاشتراك - بيانات أنواع الاشتراكات المتاحة
namespace CraftsmanAccounts.Web.Models;

public class SubscriptionType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int DurationInDays { get; set; }
    public string Details { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
