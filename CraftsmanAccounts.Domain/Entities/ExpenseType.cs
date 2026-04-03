// كيان نوع المصروف - تصنيفات المصروفات الخاصة بكل مستخدم
namespace CraftsmanAccounts.Domain.Entities;

public class ExpenseType : UserOwnedEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
