// نتيجة مُقسّمة لصفحات - تُستخدم لإرجاع بيانات مع معلومات التقسيم
namespace CraftsmanAccounts.Application.Common;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
