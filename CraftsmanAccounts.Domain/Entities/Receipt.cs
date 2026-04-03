// كيان سند القبض - يمثل عملية استلام مبلغ (عام أو مشروع)
namespace CraftsmanAccounts.Domain.Entities;

public class Receipt : UserOwnedEntity
{
    public Domain.Enums.ReceiptType ReceiptType { get; set; }
    public decimal Amount { get; set; }
    public string Details { get; set; } = string.Empty;

    public int? ClientId { get; set; }
    public Client? Client { get; set; }
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    public int? WalletId { get; set; }
    public Wallet? Wallet { get; set; }
}
