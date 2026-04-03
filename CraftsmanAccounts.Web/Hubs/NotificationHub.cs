// مركز الإشعارات الفورية - يوفر إشعارات فورية للمدير عبر SignalR
using Microsoft.AspNetCore.SignalR;

namespace CraftsmanAccounts.Web.Hubs;

/// <summary>
/// مركز الإشعارات الفورية لإرسال واستقبال الإشعارات في الوقت الحقيقي
/// </summary>
public class NotificationHub : Hub
{
    public async Task SendNotification(string title, string message, string type)
    {
        await Clients.All.SendAsync("ReceiveNotification", title, message, type);
    }
}
