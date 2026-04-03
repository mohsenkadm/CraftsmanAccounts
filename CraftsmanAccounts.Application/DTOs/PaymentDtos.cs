// كائنات نقل بيانات سندات الصرف - إنشاء وعرض مدفوعات متنوعة
namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات سند الصرف
public record PaymentDto(int Id, string PaymentTypeName, decimal Amount, string Details, int? ExpenseTypeId, string? ExpenseTypeName, int? ProjectId, string? ProjectName, int? ClientId, string? ClientName, int? WorkerId, string? WorkerName, int? WalletId, string? WalletName, DateTime CreatedAt);

// طلب إنشاء سند صرف عام
public record CreatePaymentGeneralRequest(decimal Amount, string Details, int? ExpenseTypeId, int? WalletId);

// طلب إنشاء سند صرف لمشروع
public record CreatePaymentProjectRequest(int ProjectId, decimal Amount, string Details, int? WalletId);

// طلب إنشاء سند صرف لعميل
public record CreatePaymentClientRequest(int ClientId, decimal Amount, string Details, int? WalletId);

// طلب إنشاء سند صرف لعامل
public record CreatePaymentWorkerRequest(int WorkerId, decimal Amount, string Details, int? WalletId);
