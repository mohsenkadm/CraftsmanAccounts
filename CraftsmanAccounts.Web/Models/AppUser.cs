// نموذج المستخدم - بيانات المستخدم في لوحة تحكم المدير
namespace CraftsmanAccounts.Web.Models;

public class AppUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected
}
