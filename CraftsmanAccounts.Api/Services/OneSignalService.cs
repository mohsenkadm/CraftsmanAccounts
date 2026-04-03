// خدمة إشعارات OneSignal - إرسال إشعارات الدفع للمستخدمين عبر OneSignal REST API
using System.Net.Http.Json;
using CraftsmanAccounts.Application.Interfaces;

namespace CraftsmanAccounts.Api.Services;

/// <summary>
/// تنفيذ خدمة الإشعارات باستخدام OneSignal REST API
/// ترسل إشعارات الدفع للمستخدمين عبر التطبيقات والمتصفحات
/// </summary>
public class OneSignalService : IUserNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly string _appId;
    private readonly ILogger<OneSignalService> _logger;

    public OneSignalService(HttpClient httpClient, IConfiguration configuration, ILogger<OneSignalService> logger)
    {
        _httpClient = httpClient;
        _appId = configuration["OneSignal:AppId"] ?? "";
        _logger = logger;
    }

    /// <summary>
    /// إرسال إشعار لمستخدم محدد عبر معرف المستخدم الخارجي (External User ID)
    /// </summary>
    public async Task NotifyUserAsync(int userId, string title, string message, string type)
    {
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
