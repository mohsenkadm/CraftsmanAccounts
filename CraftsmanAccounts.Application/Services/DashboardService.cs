// خدمة لوحة التحكم - الإحصائيات العامة والتحليلات البيانية للمستخدم
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Enums;
using CraftsmanAccounts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CraftsmanAccounts.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    public DashboardService(IUnitOfWork uow) => _uow = uow;

    // أسماء الأشهر بالعربية
    private static readonly string[] ArabicMonths =
    [
        "", "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
        "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
    ];

    // ترجمة فئات القيود المحاسبية للعربية
    private static string CategoryToArabic(AccountEntryCategory cat) => cat switch
    {
        AccountEntryCategory.WorkerSalary => "رواتب العمال",
        AccountEntryCategory.WorkerPayment => "دفعات العمال",
        AccountEntryCategory.ClientPayment => "دفعات العملاء",
        AccountEntryCategory.ProjectPayment => "دفعات المشاريع",
        AccountEntryCategory.ProjectExpense => "مصاريف المشاريع",
        AccountEntryCategory.GeneralExpense => "مصاريف عامة",
        AccountEntryCategory.Receipt => "سندات قبض",
        AccountEntryCategory.EquipmentLoss => "خسائر معدات",
        AccountEntryCategory.WalletTransfer => "تحويلات محافظ",
        _ => cat.ToString()
    };

    // ─── الإحصائيات الشاملة ───

    public async Task<ServiceResult<UserStatisticsDto>> GetStatisticsAsync(int userId)
    {
        // الاستعلامات المالية
        var entries = _uow.Repository<AccountEntry>().Query().Where(e => e.UserId == userId);
        var totalCredits = await entries.Where(e => e.EntryType == AccountEntryType.Credit).SumAsync(e => (decimal?)e.Amount) ?? 0;
        var totalDebits = await entries.Where(e => e.EntryType == AccountEntryType.Debit).SumAsync(e => (decimal?)e.Amount) ?? 0;
        var totalEquipmentLosses = await entries
            .Where(e => e.Category == AccountEntryCategory.EquipmentLoss)
            .SumAsync(e => (decimal?)e.Amount) ?? 0;

        // المحافظ
        var wallets = _uow.Repository<Wallet>().Query().Where(w => w.UserId == userId);
        var totalWalletBalance = await wallets.SumAsync(w => (decimal?)w.Balance) ?? 0;

        // المعدات
        var equipmentQuery = _uow.Repository<Equipment>().Query().Where(e => e.UserId == userId);
        var totalEquipmentValue = await equipmentQuery.SumAsync(e => (decimal?)(e.Amount * e.Quantity)) ?? 0;

        var financial = new FinancialSummaryDto(
            totalCredits, totalDebits, totalCredits - totalDebits,
            totalWalletBalance, totalEquipmentValue, totalEquipmentLosses);

        // الأعداد
        var projects = _uow.Repository<Project>().Query().Where(p => p.UserId == userId);
        var totalProjects = await projects.CountAsync();
        var activeProjects = await projects.CountAsync(p => p.IsActive);
        var completedProjects = totalProjects - activeProjects;

        var totalWorkers = await _uow.Repository<Worker>().Query().CountAsync(w => w.UserId == userId);
        var activeWorkers = await _uow.Repository<Worker>().Query().CountAsync(w => w.UserId == userId && w.IsActive);
        var totalClients = await _uow.Repository<Client>().Query().CountAsync(c => c.UserId == userId);

        var totalEquipment = await equipmentQuery.CountAsync();
        var damagedEquipment = await equipmentQuery.CountAsync(e => e.IsDamaged);
        var assignedQty = await _uow.Repository<ProjectEquipment>().Query()
            .Where(pe => pe.Equipment.UserId == userId && pe.Project.IsActive)
            .Select(pe => pe.EquipmentId).Distinct().CountAsync();
        var availableEquipment = totalEquipment - damagedEquipment;

        var totalWallets = await wallets.CountAsync();
        var totalReceipts = await _uow.Repository<Receipt>().Query().CountAsync(r => r.UserId == userId);
        var totalPayments = await _uow.Repository<Payment>().Query().CountAsync(p => p.UserId == userId);

        var counts = new CountsSummaryDto(
            totalProjects, activeProjects, completedProjects,
            totalWorkers, activeWorkers, totalClients,
            totalEquipment, damagedEquipment, availableEquipment,
            totalWallets, totalReceipts, totalPayments);

        return ServiceResult<UserStatisticsDto>.Ok(new UserStatisticsDto(financial, counts));
    }

    // ─── التحليلات البيانية ───

    public async Task<ServiceResult<UserAnalyticsDto>> GetAnalyticsAsync(int userId, int? year = null)
    {
        var targetYear = year ?? DateTime.UtcNow.Year;

        // 1- الحركات المالية الشهرية (مخطط خطي/شريطي)
        var monthlyFinancials = await GetMonthlyFinancialsAsync(userId, targetYear);

        // 2- توزيع المصروفات حسب الفئة (مخطط دائري)
        var expensesByCategory = await GetExpensesByCategoryAsync(userId, targetYear);

        // 3- توزيع الإيرادات حسب الفئة (مخطط دائري)
        var incomeByCategory = await GetIncomeByCategoryAsync(userId, targetYear);

        // 4- أداء المشاريع (جدول)
        var projectsPerformance = await GetProjectsPerformanceAsync(userId);

        // 5- تحليل العمال (جدول)
        var workersAnalytics = await GetWorkersAnalyticsAsync(userId);

        // 6- تحليل العملاء (جدول)
        var clientsAnalytics = await GetClientsAnalyticsAsync(userId);

        // 7- تحليل المحافظ (جدول)
        var walletsAnalytics = await GetWalletsAnalyticsAsync(userId);

        // 8- تحليل المعدات (جدول)
        var equipmentAnalytics = await GetEquipmentAnalyticsAsync(userId);

        // 9- أعلى 5 مشاريع
        var topProjects = projectsPerformance
            .OrderByDescending(p => p.TotalAmount)
            .Take(5)
            .Select(p => new TopProjectDto(p.Name, p.TotalAmount))
            .ToList();

        // 10- أعلى 5 عمال
        var topWorkers = workersAnalytics
            .OrderByDescending(w => w.TotalEarnings)
            .Take(5)
            .Select(w => new TopWorkerDto(w.Name, w.TotalEarnings))
            .ToList();

        var analytics = new UserAnalyticsDto(
            monthlyFinancials, expensesByCategory, incomeByCategory,
            projectsPerformance, workersAnalytics, clientsAnalytics,
            walletsAnalytics, equipmentAnalytics, topProjects, topWorkers);

        return ServiceResult<UserAnalyticsDto>.Ok(analytics);
    }

    // ─── الاستعلامات الفرعية ───

    private async Task<List<MonthlyFinancialDto>> GetMonthlyFinancialsAsync(int userId, int year)
    {
        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        var data = await _uow.Repository<AccountEntry>().Query()
            .Where(e => e.UserId == userId && e.CreatedAt >= startDate && e.CreatedAt <= endDate)
            .GroupBy(e => new { e.CreatedAt.Month, e.EntryType })
            .Select(g => new { g.Key.Month, g.Key.EntryType, Total = g.Sum(e => e.Amount) })
            .ToListAsync();

        var result = new List<MonthlyFinancialDto>();
        for (int m = 1; m <= 12; m++)
        {
            var credits = data.Where(d => d.Month == m && d.EntryType == AccountEntryType.Credit).Sum(d => d.Total);
            var debits = data.Where(d => d.Month == m && d.EntryType == AccountEntryType.Debit).Sum(d => d.Total);
            result.Add(new MonthlyFinancialDto(year, m, ArabicMonths[m], credits, debits, credits - debits));
        }
        return result;
    }

    private async Task<List<ExpenseByCategoryDto>> GetExpensesByCategoryAsync(int userId, int year)
    {
        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        var data = await _uow.Repository<AccountEntry>().Query()
            .Where(e => e.UserId == userId && e.EntryType == AccountEntryType.Debit
                        && e.CreatedAt >= startDate && e.CreatedAt <= endDate)
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
            .ToListAsync();

        var grandTotal = data.Sum(d => d.Total);
        return data.Select(d => new ExpenseByCategoryDto(
            d.Category.ToString(),
            CategoryToArabic(d.Category),
            d.Total,
            grandTotal > 0 ? Math.Round(d.Total / grandTotal * 100, 2) : 0
        )).OrderByDescending(d => d.Amount).ToList();
    }

    private async Task<List<IncomeByCategoryDto>> GetIncomeByCategoryAsync(int userId, int year)
    {
        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        var data = await _uow.Repository<AccountEntry>().Query()
            .Where(e => e.UserId == userId && e.EntryType == AccountEntryType.Credit
                        && e.CreatedAt >= startDate && e.CreatedAt <= endDate)
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
            .ToListAsync();

        var grandTotal = data.Sum(d => d.Total);
        return data.Select(d => new IncomeByCategoryDto(
            d.Category.ToString(),
            CategoryToArabic(d.Category),
            d.Total,
            grandTotal > 0 ? Math.Round(d.Total / grandTotal * 100, 2) : 0
        )).OrderByDescending(d => d.Amount).ToList();
    }

    private async Task<List<ProjectPerformanceDto>> GetProjectsPerformanceAsync(int userId)
    {
        return await _uow.Repository<Project>().Query()
            .Where(p => p.UserId == userId)
            .Select(p => new ProjectPerformanceDto(
                p.Id, p.Name,
                p.Client != null ? p.Client.Name : null,
                p.TotalAmount, p.PaidAmount,
                p.TotalAmount - p.PaidAmount,
                p.AccountEntries.Where(e => e.EntryType == AccountEntryType.Debit).Sum(e => (decimal?)e.Amount) ?? 0,
                p.PaidAmount - (p.AccountEntries.Where(e => e.EntryType == AccountEntryType.Debit).Sum(e => (decimal?)e.Amount) ?? 0),
                p.TotalAmount > 0 ? Math.Round((double)(p.PaidAmount / p.TotalAmount) * 100, 1) : 0,
                p.ProjectWorkers.Count,
                p.ProjectEquipments.Count,
                p.IsActive))
            .OrderByDescending(p => p.TotalAmount)
            .ToListAsync();
    }

    private async Task<List<WorkerAnalyticsDto>> GetWorkersAnalyticsAsync(int userId)
    {
        return await _uow.Repository<Worker>().Query()
            .Where(w => w.UserId == userId)
            .Select(w => new WorkerAnalyticsDto(
                w.Id, w.Name,
                w.DailyEntries.Sum(de => (decimal?)(de.DailyRate + de.ExtraAmount)) ?? 0,
                w.DailyEntries.Sum(de => (decimal?)de.ExtraAmount) ?? 0,
                w.DailyEntries.Select(de => de.WorkerDaily.Date).Distinct().Count(),
                w.ProjectWorkers.Count,
                w.DailyEntries.Any() ? w.DailyEntries.Average(de => (decimal?)de.DailyRate) ?? 0 : 0,
                w.IsActive))
            .OrderByDescending(w => w.TotalEarnings)
            .ToListAsync();
    }

    private async Task<List<ClientAnalyticsDto>> GetClientsAnalyticsAsync(int userId)
    {
        return await _uow.Repository<Client>().Query()
            .Where(c => c.UserId == userId)
            .Select(c => new ClientAnalyticsDto(
                c.Id, c.Name,
                c.ClientType.ToString(),
                c.Projects.Count,
                c.Projects.Sum(p => (decimal?)p.TotalAmount) ?? 0,
                c.Projects.Sum(p => (decimal?)p.PaidAmount) ?? 0,
                c.Projects.Sum(p => (decimal?)(p.TotalAmount - p.PaidAmount)) ?? 0,
                c.IsActive))
            .OrderByDescending(c => c.TotalProjectsValue)
            .ToListAsync();
    }

    private async Task<List<WalletAnalyticsDto>> GetWalletsAnalyticsAsync(int userId)
    {
        return await _uow.Repository<Wallet>().Query()
            .Where(w => w.UserId == userId)
            .Select(w => new WalletAnalyticsDto(
                w.Id, w.Name, w.Balance,
                w.User.AccountEntries.Where(e => e.WalletId == w.Id && e.EntryType == AccountEntryType.Credit).Sum(e => (decimal?)e.Amount) ?? 0,
                w.User.AccountEntries.Where(e => e.WalletId == w.Id && e.EntryType == AccountEntryType.Debit).Sum(e => (decimal?)e.Amount) ?? 0))
            .ToListAsync();
    }

    private async Task<List<EquipmentAnalyticsDto>> GetEquipmentAnalyticsAsync(int userId)
    {
        return await _uow.Repository<Equipment>().Query()
            .Where(e => e.UserId == userId)
            .Select(e => new EquipmentAnalyticsDto(
                e.Id, e.Name, e.Amount, e.Quantity,
                e.Quantity - e.ProjectEquipments.Where(pe => pe.Project.IsActive).Sum(pe => pe.Quantity),
                e.ProjectEquipments.Where(pe => pe.Project.IsActive).Sum(pe => pe.Quantity),
                e.IsDamaged,
                e.ProjectEquipments.Select(pe => pe.ProjectId).Distinct().Count()))
            .OrderByDescending(e => e.Amount * e.Quantity)
            .ToListAsync();
    }
}
