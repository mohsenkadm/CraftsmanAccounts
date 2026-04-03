// الكيان الأساسي - يحتوي على المعرف وتاريخ الإنشاء لجميع الكيانات
namespace CraftsmanAccounts.Domain.Entities;

/// <summary>الكيان الأساسي الذي ترثه جميع الكيانات</summary>
public abstract class BaseEntity
{
    /// <summary>المعرف الفريد</summary>
    public int Id { get; set; }

    /// <summary>تاريخ الإنشاء</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
