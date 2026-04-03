// كائنات نقل بيانات المحافظ المالية
namespace CraftsmanAccounts.Application.DTOs;

// عرض بيانات المحفظة
public record WalletDto(int Id, string Name, decimal Balance);

// طلب إنشاء محفظة جديدة
public record CreateWalletRequest(string Name, decimal InitialBalance);
