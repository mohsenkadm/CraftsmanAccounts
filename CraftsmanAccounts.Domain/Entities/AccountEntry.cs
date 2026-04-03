// كيان القيد المحاسبي - يسجل جميع الحركات المالية للكشوفات والأرباح والخسائر
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Domain.Entities;

public class AccountEntry : UserOwnedEntity
{
    public AccountEntryType EntryType { get; set; }
    public AccountEntryCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;

    public int? WorkerId { get; set; }
    public Worker? Worker { get; set; }
    public int? ClientId { get; set; }
    public Client? Client { get; set; }
    public int? ProjectId { get; set; }
    public Project? Project { get; set; }
    public int? WalletId { get; set; }
    public Wallet? Wallet { get; set; }
}
