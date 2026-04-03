// خدمة اشتراكات المستخدمين الوهمية - بيانات تجريبية للاشتراكات للاختبار
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Models.ViewModels;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Services.Mock;

public class MockUserSubscriptionService : IUserSubscriptionService
{
    private static readonly List<UserSubscription> Subscriptions =
    [
        new() { Id = 1, UserId = 1, UserName = "أحمد محمد علي", SubscriptionTypeId = 1, SubscriptionTypeName = "الاشتراك الشهري", Amount = 25000, StartDate = DateTime.Now.AddDays(-20), EndDate = DateTime.Now.AddDays(10), Status = SubscriptionStatus.Paid, IsPaid = true },
        new() { Id = 2, UserId = 2, UserName = "سارة حسن كاظم", SubscriptionTypeId = 2, SubscriptionTypeName = "الاشتراك ربع السنوي", Amount = 60000, StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(80), Status = SubscriptionStatus.Approved, IsPaid = false },
        new() { Id = 3, UserId = 6, UserName = "زينب محمد صالح", SubscriptionTypeId = 4, SubscriptionTypeName = "الاشتراك السنوي", Amount = 180000, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(360), Status = SubscriptionStatus.Pending, IsPaid = false },
        new() { Id = 4, UserId = 9, UserName = "مصطفى جعفر كريم", SubscriptionTypeId = 1, SubscriptionTypeName = "الاشتراك الشهري", Amount = 25000, StartDate = DateTime.Now.AddDays(-3), EndDate = DateTime.Now.AddDays(27), Status = SubscriptionStatus.Pending, IsPaid = false },
        new() { Id = 5, UserId = 11, UserName = "عمر سعد محمود", SubscriptionTypeId = 3, SubscriptionTypeName = "الاشتراك نصف السنوي", Amount = 100000, StartDate = DateTime.Now.AddDays(-15), EndDate = DateTime.Now.AddDays(165), Status = SubscriptionStatus.Paid, IsPaid = true },
        new() { Id = 6, UserId = 1, UserName = "أحمد محمد علي", SubscriptionTypeId = 4, SubscriptionTypeName = "الاشتراك السنوي", Amount = 180000, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(364), Status = SubscriptionStatus.Pending, IsPaid = false }
    ];

    public UserSubscriptionFilterViewModel GetSubscriptions(UserSubscriptionFilterViewModel filter)
    {
        var query = Subscriptions.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(s =>
                s.UserName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                s.SubscriptionTypeName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.Status.HasValue)
            query = query.Where(s => s.Status == filter.Status.Value);

        query = query.OrderByDescending(s => s.StartDate);

        var totalCount = query.Count();
        var subscriptions = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToList();

        return new UserSubscriptionFilterViewModel
        {
            SearchTerm = filter.SearchTerm,
            Status = filter.Status,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = totalCount,
            Subscriptions = subscriptions
        };
    }

    public UserSubscription? GetById(int id) => Subscriptions.FirstOrDefault(s => s.Id == id);

    public void Approve(int id)
    {
        var sub = Subscriptions.FirstOrDefault(s => s.Id == id);
        if (sub is null) return;
        sub.Status = SubscriptionStatus.Approved;
    }

    public void Reject(int id)
    {
        var sub = Subscriptions.FirstOrDefault(s => s.Id == id);
        if (sub is null) return;
        sub.Status = SubscriptionStatus.Rejected;
    }

    public void MarkAsPaid(int id)
    {
        var sub = Subscriptions.FirstOrDefault(s => s.Id == id);
        if (sub is null) return;
        sub.Status = SubscriptionStatus.Paid;
        sub.IsPaid = true;
    }

    public int GetTotalCount() => Subscriptions.Count;
    public int GetActiveCount() => Subscriptions.Count(s => s.Status == SubscriptionStatus.Paid || s.Status == SubscriptionStatus.Approved);
    public decimal GetTotalRevenue() => Subscriptions.Where(s => s.IsPaid).Sum(s => s.Amount);
    public List<UserSubscription> GetRecent(int count) => Subscriptions.OrderByDescending(s => s.StartDate).Take(count).ToList();
}
