// خدمة المستخدمين الوهمية - بيانات تجريبية للمستخدمين للاختبار
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Models.ViewModels;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Services.Mock;

public class MockUserService : IUserService
{
    private static readonly List<AppUser> Users =
    [
        new() { Id = 1, FullName = "أحمد محمد علي", Address = "بغداد - الكرادة", PhoneNumber = "07701234567", IsActive = true, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.Now.AddDays(-30) },
        new() { Id = 2, FullName = "سارة حسن كاظم", Address = "بغداد - المنصور", PhoneNumber = "07709876543", IsActive = true, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.Now.AddDays(-25) },
        new() { Id = 3, FullName = "محمد عبدالله جاسم", Address = "البصرة - العشار", PhoneNumber = "07801112233", IsActive = false, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.Now.AddDays(-20) },
        new() { Id = 4, FullName = "فاطمة عبدالرحمن", Address = "أربيل - عينكاوا", PhoneNumber = "07504445566", IsActive = true, ApprovalStatus = ApprovalStatus.Pending, CreatedAt = DateTime.Now.AddDays(-5) },
        new() { Id = 5, FullName = "علي كريم حسين", Address = "النجف - حي السعد", PhoneNumber = "07707778899", IsActive = true, ApprovalStatus = ApprovalStatus.Pending, CreatedAt = DateTime.Now.AddDays(-3) },
        new() { Id = 6, FullName = "زينب محمد صالح", Address = "كربلاء - حي العباس", PhoneNumber = "07811223344", IsActive = true, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.Now.AddDays(-15) },
        new() { Id = 7, FullName = "حسين علي ناصر", Address = "بغداد - الأعظمية", PhoneNumber = "07705556677", IsActive = true, ApprovalStatus = ApprovalStatus.Pending, CreatedAt = DateTime.Now.AddDays(-1) },
        new() { Id = 8, FullName = "نور حيدر عباس", Address = "بغداد - زيونة", PhoneNumber = "07708889900", IsActive = false, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.Now.AddDays(-10) },
        new() { Id = 9, FullName = "مصطفى جعفر كريم", Address = "الموصل - الدواسة", PhoneNumber = "07501234567", IsActive = true, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.Now.AddDays(-8) },
        new() { Id = 10, FullName = "رقية أحمد حسن", Address = "بابل - الحلة", PhoneNumber = "07809876543", IsActive = true, ApprovalStatus = ApprovalStatus.Pending, CreatedAt = DateTime.Now.AddDays(-2) },
        new() { Id = 11, FullName = "عمر سعد محمود", Address = "ديالى - بعقوبة", PhoneNumber = "07706667788", IsActive = true, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.Now.AddDays(-12) },
        new() { Id = 12, FullName = "هدى كاظم جواد", Address = "واسط - الكوت", PhoneNumber = "07812345678", IsActive = false, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.Now.AddDays(-18) }
    ];

    public UserFilterViewModel GetUsers(UserFilterViewModel filter)
    {
        var query = Users.Where(u => u.ApprovalStatus != ApprovalStatus.Rejected).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(u =>
                u.FullName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                u.PhoneNumber.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                u.Address.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.IsActive.HasValue)
            query = query.Where(u => u.IsActive == filter.IsActive.Value);

        query = filter.SortBy switch
        {
            "name" => filter.SortDescending ? query.OrderByDescending(u => u.FullName) : query.OrderBy(u => u.FullName),
            "date" => filter.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            "status" => filter.SortDescending ? query.OrderByDescending(u => u.IsActive) : query.OrderBy(u => u.IsActive),
            _ => query.OrderByDescending(u => u.CreatedAt)
        };

        var totalCount = query.Count();
        var users = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();

        return new UserFilterViewModel
        {
            SearchTerm = filter.SearchTerm,
            IsActive = filter.IsActive,
            SortBy = filter.SortBy,
            SortDescending = filter.SortDescending,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount,
            Users = users
        };
    }

    public List<AppUser> GetPendingUsers() =>
        Users.Where(u => u.ApprovalStatus == ApprovalStatus.Pending).OrderByDescending(u => u.CreatedAt).ToList();

    public AppUser? GetById(int id) => Users.FirstOrDefault(u => u.Id == id);

    public void Update(AppUser user)
    {
        var existing = Users.FirstOrDefault(u => u.Id == user.Id);
        if (existing is null) return;

        existing.FullName = user.FullName;
        existing.Address = user.Address;
        existing.PhoneNumber = user.PhoneNumber;
        existing.IsActive = user.IsActive;
    }

    public void Delete(int id) => Users.RemoveAll(u => u.Id == id);

    public void Approve(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user is null) return;
        user.ApprovalStatus = ApprovalStatus.Approved;
        user.IsActive = true;
    }

    public void Reject(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user is null) return;
        user.ApprovalStatus = ApprovalStatus.Rejected;
        user.IsActive = false;
    }

    public void ToggleActive(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user is null) return;
        user.IsActive = !user.IsActive;
    }

    public int GetTotalCount() => Users.Count(u => u.ApprovalStatus != ApprovalStatus.Rejected);
    public int GetActiveCount() => Users.Count(u => u.IsActive && u.ApprovalStatus == ApprovalStatus.Approved);
    public int GetPendingCount() => Users.Count(u => u.ApprovalStatus == ApprovalStatus.Pending);
}
