// كيان المعدات - يمثل معدة مشتراة مع إمكانية تسجيل التلف وتتبع الكمية
namespace CraftsmanAccounts.Domain.Entities;

public class Equipment : UserOwnedEntity
{
    public string Name { get; set; } = string.Empty;
    public string PurchasedFrom { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsDamaged { get; set; }

    public ICollection<ProjectEquipment> ProjectEquipments { get; set; } = new List<ProjectEquipment>();
}
