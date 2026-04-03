// كائنات نقل بيانات سندات القبض - إنشاء وعرض سندات القبض العامة وللمشاريع
namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات سند القبض
public record ReceiptDto(int Id, string ReceiptTypeName, decimal Amount, string Details, int? ClientId, string? ClientName, int? ProjectId, string? ProjectName, int? WalletId, string? WalletName, DateTime CreatedAt);

// طلب إنشاء سند قبض عام
public record CreateReceiptGeneralRequest(decimal Amount, string Details, int? ClientId, int? WalletId);

// طلب إنشاء سند قبض لمشروع
public record CreateReceiptProjectRequest(int ProjectId, decimal Amount, string Details, int? WalletId);
