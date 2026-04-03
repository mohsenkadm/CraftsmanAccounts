// كيان الإشعار - يسجل جميع الإشعارات المرسلة لكل مستخدم
namespace CraftsmanAccounts.Domain.Entities;

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}
