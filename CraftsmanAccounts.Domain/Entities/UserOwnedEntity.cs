// الكيان الأساسي المملوك من المستخدم - يرث من الكيان الأساسي ويضيف معرف المستخدم
namespace CraftsmanAccounts.Domain.Entities;

public abstract class UserOwnedEntity : BaseEntity
{
    /// <summary>معرف المستخدم المالك</summary>
    public int UserId { get; set; }

    /// <summary>المستخدم المالك</summary>
    public AppUser User { get; set; } = null!;
}
