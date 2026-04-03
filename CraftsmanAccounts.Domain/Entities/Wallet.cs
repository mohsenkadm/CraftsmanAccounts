// كيان المحفظة - تمثل محفظة مالية برصيد يتم تحديثه تلقائياً
namespace CraftsmanAccounts.Domain.Entities;

public class Wallet : UserOwnedEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
