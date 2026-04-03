// كائنات نقل بيانات اليوميات - سجل يومي لأجور العمال في المشاريع
namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات اليومية
public record WorkerDailyDto(int Id, int ProjectId, string ProjectName, DateTime Date, bool IsPaid, List<WorkerDailyEntryDto> Entries);

// عرض تفاصيل سطر في اليومية
public record WorkerDailyEntryDto(int Id, int WorkerId, string WorkerName, decimal DailyRate, decimal ExtraAmount);

// طلب إنشاء يومية جديدة
public record CreateWorkerDailyRequest(int ProjectId, DateTime Date, int? WalletId, List<WorkerDailyEntryItem> Entries);

// عنصر إدخال يومية - معرف العامل والمبلغ الإضافي
public record WorkerDailyEntryItem(int WorkerId, decimal ExtraAmount);
