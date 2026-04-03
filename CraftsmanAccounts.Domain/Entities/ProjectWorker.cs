// كيان عامل المشروع - يربط العامل بالمشروع مع الأجرة اليومية
namespace CraftsmanAccounts.Domain.Entities;

public class ProjectWorker : BaseEntity
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int WorkerId { get; set; }
    public Worker Worker { get; set; } = null!;
    public decimal DailyRate { get; set; }
}
