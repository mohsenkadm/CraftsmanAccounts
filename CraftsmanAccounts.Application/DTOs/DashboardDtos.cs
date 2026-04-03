// كائنات نقل بيانات لوحة التحكم - الإحصائيات العامة والتحليلات البيانية والجداول
namespace CraftsmanAccounts.Application.DTOs;

// ─── الإحصائيات العامة للمستخدم ───

// ملخص مالي عام
public record FinancialSummaryDto(
    decimal TotalCredits,
    decimal TotalDebits,
    decimal NetBalance,
    decimal TotalWalletBalance,
    decimal TotalEquipmentValue,
    decimal TotalEquipmentLosses);

// ملخص الأعداد والكميات
public record CountsSummaryDto(
    int TotalProjects,
    int ActiveProjects,
    int CompletedProjects,
    int TotalWorkers,
    int ActiveWorkers,
    int TotalClients,
    int TotalEquipment,
    int DamagedEquipment,
    int AvailableEquipment,
    int TotalWallets,
    int TotalReceipts,
    int TotalPayments);

// إحصائيات المستخدم الشاملة
public record UserStatisticsDto(
    FinancialSummaryDto Financial,
    CountsSummaryDto Counts);

// ─── تحليل البيانات للمخططات البيانية ───

// حركة مالية شهرية (للمخطط البياني الخطي/الشريطي)
public record MonthlyFinancialDto(
    int Year,
    int Month,
    string MonthName,
    decimal Credits,
    decimal Debits,
    decimal Net);

// توزيع المصروفات حسب الفئة (للمخطط الدائري)
public record ExpenseByCategoryDto(
    string Category,
    string CategoryArabic,
    decimal Amount,
    decimal Percentage);

// توزيع الإيرادات حسب الفئة (للمخطط الدائري)
public record IncomeByCategoryDto(
    string Category,
    string CategoryArabic,
    decimal Amount,
    decimal Percentage);

// أداء المشروع (للجدول والمخطط الشريطي)
public record ProjectPerformanceDto(
    int Id,
    string Name,
    string? ClientName,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal RemainingAmount,
    decimal TotalExpenses,
    decimal Profit,
    double ProgressPercentage,
    int WorkerCount,
    int EquipmentCount,
    bool IsActive);

// إحصائيات العمال (للجدول)
public record WorkerAnalyticsDto(
    int Id,
    string Name,
    decimal TotalEarnings,
    decimal TotalExtraAmount,
    int WorkDays,
    int ProjectCount,
    decimal AverageDailyRate,
    bool IsActive);

// إحصائيات العملاء (للجدول)
public record ClientAnalyticsDto(
    int Id,
    string Name,
    string ClientType,
    int ProjectCount,
    decimal TotalProjectsValue,
    decimal TotalPaid,
    decimal TotalRemaining,
    bool IsActive);

// أداء المحافظ (للجدول)
public record WalletAnalyticsDto(
    int Id,
    string Name,
    decimal Balance,
    decimal TotalCredits,
    decimal TotalDebits);

// إحصائيات المعدات (للجدول)
public record EquipmentAnalyticsDto(
    int Id,
    string Name,
    decimal Amount,
    int Quantity,
    int AvailableQuantity,
    int AssignedQuantity,
    bool IsDamaged,
    int ProjectCount);

// أعلى 5 مشاريع من حيث القيمة
public record TopProjectDto(string Name, decimal TotalAmount);

// أعلى 5 عمال من حيث الأرباح
public record TopWorkerDto(string Name, decimal TotalEarnings);

// استجابة التحليلات الشاملة
public record UserAnalyticsDto(
    List<MonthlyFinancialDto> MonthlyFinancials,
    List<ExpenseByCategoryDto> ExpensesByCategory,
    List<IncomeByCategoryDto> IncomeByCategory,
    List<ProjectPerformanceDto> ProjectsPerformance,
    List<WorkerAnalyticsDto> WorkersAnalytics,
    List<ClientAnalyticsDto> ClientsAnalytics,
    List<WalletAnalyticsDto> WalletsAnalytics,
    List<EquipmentAnalyticsDto> EquipmentAnalytics,
    List<TopProjectDto> TopProjects,
    List<TopWorkerDto> TopWorkers);
