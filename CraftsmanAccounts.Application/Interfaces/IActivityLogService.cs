// واجهة خدمة سجل النشاطات - تسجيل وجلب نشاطات المستخدم
using CraftsmanAccounts.Application.Common;
using CraftsmanAccounts.Application.DTOs;

namespace CraftsmanAccounts.Application.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(int userId, string action, string entityName, int? entityId, string? details, string? ipAddress);
    Task<ServiceResult<PagedResult<ActivityLogDto>>> GetLogsAsync(int userId, ActivityLogRequest request);
}
