// كيان تفاصيل اليومية - سطر واحد لعامل في يومية معينة
namespace CraftsmanAccounts.Domain.Entities;

public class WorkerDailyEntry : BaseEntity
{
    public int WorkerDailyId { get; set; }
    public WorkerDaily WorkerDaily { get; set; } = null!;
    public int WorkerId { get; set; }
    public Worker Worker { get; set; } = null!;
    public decimal DailyRate { get; set; }
    public decimal ExtraAmount { get; set; }
}
