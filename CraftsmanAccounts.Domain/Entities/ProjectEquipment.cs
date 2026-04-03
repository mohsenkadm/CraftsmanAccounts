// كيان معدة المشروع - يربط المعدة بالمشروع مع الكمية المستخدمة
namespace CraftsmanAccounts.Domain.Entities;

public class ProjectEquipment : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;
    public int Quantity { get; set; }
}
