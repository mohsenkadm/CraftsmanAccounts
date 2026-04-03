// كيان سجل نشاطات المستخدم - يسجل كل عملية يقوم بها المستخدم
namespace CraftsmanAccounts.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public string Action { get; set; } = string.Empty;      // نوع العملية: إضافة، تعديل، حذف، إلخ
    public string EntityName { get; set; } = string.Empty;   // اسم الكيان: مشروع، عامل، عميل، إلخ
    public int? EntityId { get; set; }                       // معرف الكيان المتأثر
    public string? Details { get; set; }                     // تفاصيل إضافية
    public string? IpAddress { get; set; }                   // عنوان IP
}
