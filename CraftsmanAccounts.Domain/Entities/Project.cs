// كيان المشروع - يمثل مشروع تابع لمستخدم ومرتبط بعميل
namespace CraftsmanAccounts.Domain.Entities;

public class Project : UserOwnedEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public ICollection<ProjectWorker> ProjectWorkers { get; set; } = new List<ProjectWorker>();
    public ICollection<ProjectEquipment> ProjectEquipments { get; set; } = new List<ProjectEquipment>();
    public ICollection<WorkerDaily> WorkerDailies { get; set; } = new List<WorkerDaily>();
    public ICollection<AccountEntry> AccountEntries { get; set; } = new List<AccountEntry>();
}
