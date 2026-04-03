// نموذج فلتر المستخدمين - بحث وتصفية وترقيم صفحات المستخدمين
namespace CraftsmanAccounts.Web.Models.ViewModels;

public class UserFilterViewModel
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public List<AppUser> Users { get; set; } = new();
}
