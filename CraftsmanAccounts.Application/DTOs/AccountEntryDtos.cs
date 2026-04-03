// كائنات نقل بيانات القيود المحاسبية والكشوفات والأرباح والخسائر
using CraftsmanAccounts.Application.Common;

namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات القيد المحاسبي
public record AccountEntryDto(int Id, string EntryType, string Category, decimal Amount, string Description, DateTime CreatedAt, int? WorkerId, string? WorkerName, int? ClientId, string? ClientName, int? ProjectId, string? ProjectName);

// طلب كشف حساب - يمتد من طلب الصفحات مع فلترة التاريخ ونوع القيد والتصنيف
public class AccountStatementRequest : PagedRequest
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? EntryType { get; set; }   // Credit أو Debit
    public string? Category { get; set; }    // تصنيف القيد
}

// كشف حساب مع ملخص الإجماليات
public record AccountStatementDto(
    decimal TotalCredits,
    decimal TotalDebits,
    decimal Balance,
    PagedResult<AccountEntryDto> Entries);

// عرض تقرير الأرباح والخسائر
public record ProfitLossDto(decimal TotalReceipts, decimal TotalPayments, decimal EquipmentLosses, decimal NetProfit, List<AccountEntryDto> Entries);

// عرض الحركات المالية اليومية
public record DailyMovementsDto(DateTime Date, decimal TotalCredits, decimal TotalDebits, decimal Balance, List<AccountEntryDto> Entries);
