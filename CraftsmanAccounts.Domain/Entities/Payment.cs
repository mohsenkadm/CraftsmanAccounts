// كيان سند الصرف - يمثل عملية صرف مبلغ (عام أو مشروع أو عميل أو عامل)
namespace CraftsmanAccounts.Domain.Entities;

public class Payment : UserOwnedEntity
{
    public Domain.Enums.PaymentType PaymentType { get; set; }
    public decimal Amount { get; set; }
    public string Details { get; set; } = string.Empty;

    public int? ExpenseTypeId { get; set; }
    public ExpenseType? ExpenseType { get; set; }
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    public int? ClientId { get; set; }
    public Client? Client { get; set; }
    public int? WorkerId { get; set; }
    public Worker? Worker { get; set; }
    public int? WalletId { get; set; }
    public Wallet? Wallet { get; set; }
}
