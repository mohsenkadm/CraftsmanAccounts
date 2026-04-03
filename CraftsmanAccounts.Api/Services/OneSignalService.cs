// خدمة إشعارات OneSignal - إرسال إشعارات الدفع للمستخدمين عبر OneSignal REST API
using System.Net.Http.Json;
using CraftsmanAccounts.Application.Interfaces;
using CraftsmanAccounts.Domain.Entities;
using CraftsmanAccounts.Domain.Interfaces;

namespace CraftsmanAccounts.Api.Services;

/// <summary>
/// تنفيذ خدمة الإشعارات باستخدام OneSignal REST API
/// ترسل إشعارات الدفع للمستخدمين عبر التطبيقات والمتصفحات وتحفظها في قاعدة البيانات
/// </summary>
public class OneSignalService : IUserNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly string _appId;
    private readonly ILogger<OneSignalService> _logger;
    private readonly IUnitOfWork _uow;

    public OneSignalService(HttpClient httpClient, IConfiguration configuration, ILogger<OneSignalService> logger, IUnitOfWork uow)
    {
        _httpClient = httpClient;
        _appId = configuration["OneSignal:AppId"] ?? "";
        _logger = logger;
        _uow = uow;
    }

    /// <summary>
    /// إرسال إشعار لمستخدم محدد عبر معرف المستخدم الخارجي (External User ID)
    /// </summary>
    public async Task NotifyUserAsync(int userId, string title, string message, string type)
    {
        // حفظ الإشعار في قاعدة البيانات
        try
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false
            };
            await _uow.Repository<Notification>().AddAsync(notification);
            await _uow.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حفظ الإشعار في قاعدة البيانات للمستخدم {UserId}", userId);
        }

        // إرسال الإشعار عبر OneSignal
        try
        {
            var payload = new
            {
                app_id = _appId,
                include_aliases = new { external_id = new[] { userId.ToString() } },
                target_channel = "push",
                headings = new { ar = title, en = title },
                contents = new { ar = message, en = message },
                data = new { type, userId }
            };

            var response = await _httpClient.PostAsJsonAsync("notifications", payload);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("فشل إرسال إشعار OneSignal للمستخدم {UserId}: {Error}", userId, error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إرسال إشعار OneSignal للمستخدم {UserId}", userId);
        }
    }

    /// <summary>
    /// إرسال إشعار لجميع المستخدمين المشتركين في التطبيق
    /// </summary>
    public async Task NotifyAllAsync(string title, string message, string type)
    {
        // حفظ الإشعار لجميع المستخدمين في قاعدة البيانات
        try
        {
            var users = await _uow.Repository<AppUser>().FindAsync(u => u.IsActive);
            var notifications = users.Select(user => new Notification
            {
                UserId = user.Id,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false
            }).ToList();

            await _uow.Repository<Notification>().AddRangeAsync(notifications);
            await _uow.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حفظ الإشعارات في قاعدة البيانات لجميع المستخدمين");
        }

        // إرسال الإشعار عبر OneSignal
        try
        {
            var payload = new
            {
                app_id = _appId,
                included_segments = new[] { "All" },
                headings = new { ar = title, en = title },
                contents = new { ar = message, en = message },
                data = new { type }
            };

            var response = await _httpClient.PostAsJsonAsync("notifications", payload);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("فشل إرسال إشعار OneSignal للجميع: {Error}", error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إرسال إشعار OneSignal للجميع");
        }
    }
}
