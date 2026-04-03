// نموذج الخطأ - عرض معلومات الخطأ في صفحة الخطأ
namespace CraftsmanAccounts.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
