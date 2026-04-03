// تصنيف القيد المحاسبي
namespace CraftsmanAccounts.Domain.Enums;

public enum AccountEntryCategory
{
    WorkerSalary,    // راتب عامل
    WorkerPayment,   // دفعة عامل
    ClientPayment,   // دفعة عميل
    ProjectPayment,  // دفعة مشروع
    ProjectExpense,  // مصروف مشروع
    GeneralExpense,  // مصروف عام
    Receipt,         // سند قبض
    EquipmentLoss,   // خسارة معدات
    WalletTransfer   // تحويل محفظة
}
