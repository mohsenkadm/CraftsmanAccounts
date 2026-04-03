// طلب مُقسّم لصفحات - يحتوي على معايير البحث والترتيب والتقسيم
namespace CraftsmanAccounts.Application.Common;

public class PagedRequest
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}
