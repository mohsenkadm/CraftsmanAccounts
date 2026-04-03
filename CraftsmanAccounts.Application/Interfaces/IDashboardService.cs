// واجهة خدمة لوحة التحكم - الإحصائيات العامة والتحليلات البيانية للمستخدم
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IDashboardService
{
    // إحصائيات المستخدم الشاملة (أرقام وملخصات)
    Task<ServiceResult<UserStatisticsDto>> GetStatisticsAsync(int userId);

    // تحليل البيانات للمخططات البيانية والجداول
    Task<ServiceResult<UserAnalyticsDto>> GetAnalyticsAsync(int userId, int? year = null);
}
