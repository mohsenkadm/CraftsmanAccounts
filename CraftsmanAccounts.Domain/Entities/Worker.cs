// كيان العامل - يمثل عامل تابع لمستخدم معين
namespace CraftsmanAccounts.Domain.Entities;

public class Worker : UserOwnedEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<ProjectWorker> ProjectWorkers { get; set; } = new List<ProjectWorker>();
    public ICollection<WorkerDailyEntry> DailyEntries { get; set; } = new List<WorkerDailyEntry>();
    public ICollection<AccountEntry> AccountEntries { get; set; } = new List<AccountEntry>();
}
