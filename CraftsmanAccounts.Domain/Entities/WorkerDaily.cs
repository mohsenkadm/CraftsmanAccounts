// كيان اليومية - سجل يومي لأجور العمال في مشروع معين
namespace CraftsmanAccounts.Domain.Entities;

public class WorkerDaily : UserOwnedEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool IsPaid { get; set; }
    public int? WalletId { get; set; }
    public Wallet? Wallet { get; set; }

    public ICollection<WorkerDailyEntry> Entries { get; set; } = new List<WorkerDailyEntry>();
}
