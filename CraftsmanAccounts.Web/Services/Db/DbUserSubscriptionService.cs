// خدمة اشتراكات المستخدمين المدعومة بقاعدة البيانات - إدارة الاشتراكات والموافقات والمدفوعات
using CraftsmanAccounts.Domain.Interfaces;
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Models.ViewModels;
using CraftsmanAccounts.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DomainUS = CraftsmanAccounts.Domain.Entities.UserSubscription;
using DomainStatus = CraftsmanAccounts.Domain.Enums.SubscriptionStatus;

namespace CraftsmanAccounts.Web.Services.Db;

public class DbUserSubscriptionService : IUserSubscriptionService
{
    private readonly IUnitOfWork _uow;
    public DbUserSubscriptionService(IUnitOfWork uow) => _uow = uow;

    public UserSubscriptionFilterViewModel GetSubscriptions(UserSubscriptionFilterViewModel filter)
    {
        var q = _uow.Repository<DomainUS>().Query()
            .Include(us => us.User)
            .Include(us => us.SubscriptionType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            q = q.Where(us => us.User.FullName.Contains(filter.SearchTerm));
        if (filter.Status.HasValue)
            q = q.Where(us => (int)us.Status == (int)filter.Status.Value);

        filter.TotalCount = q.Count();
        filter.Subscriptions = q
            .OrderByDescending(us => us.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(us => new UserSubscription
            {
                Id = us.Id, UserId = us.UserId, UserName = us.User.FullName,
                SubscriptionTypeId = us.SubscriptionTypeId, SubscriptionTypeName = us.SubscriptionType.Name,
                Amount = us.Amount, StartDate = us.StartDate, EndDate = us.EndDate,
                Status = (SubscriptionStatus)(int)us.Status, IsPaid = us.IsPaid
            })
            .ToList();

        return filter;
    }

    public UserSubscription? GetById(int id)
    {
        var us = _uow.Repository<DomainUS>().Query()
            .Include(x => x.User).Include(x => x.SubscriptionType)
            .FirstOrDefault(x => x.Id == id);
        if (us == null) return null;
        return new UserSubscription
        {
            Id = us.Id, UserId = us.UserId, UserName = us.User.FullName,
            SubscriptionTypeId = us.SubscriptionTypeId, SubscriptionTypeName = us.SubscriptionType.Name,
            Amount = us.Amount, StartDate = us.StartDate, EndDate = us.EndDate,
            Status = (SubscriptionStatus)(int)us.Status, IsPaid = us.IsPaid
        };
    }

    public void Approve(int id)
    {
        var us = _uow.Repository<DomainUS>().Query().FirstOrDefault(x => x.Id == id);
        if (us == null) return;
        us.Status = DomainStatus.Approved;
        _uow.Repository<DomainUS>().Update(us);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public void Reject(int id)
    {
        var us = _uow.Repository<DomainUS>().Query().FirstOrDefault(x => x.Id == id);
        if (us == null) return;
        us.Status = DomainStatus.Rejected;
        _uow.Repository<DomainUS>().Update(us);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public void MarkAsPaid(int id)
    {
        var us = _uow.Repository<DomainUS>().Query().FirstOrDefault(x => x.Id == id);
        if (us == null) return;
        us.IsPaid = true;
        us.Status = DomainStatus.Paid;
        _uow.Repository<DomainUS>().Update(us);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public int GetTotalCount() => _uow.Repository<DomainUS>().Query().Count();
    public int GetActiveCount() => _uow.Repository<DomainUS>().Query().Count(us => us.Status == DomainStatus.Approved || us.Status == DomainStatus.Paid);
    public decimal GetTotalRevenue() => _uow.Repository<DomainUS>().Query().Where(x => x.IsPaid).Sum(x => x.Amount);
    public List<UserSubscription> GetRecent(int count)
    {
        return _uow.Repository<DomainUS>().Query()
            .Include(x => x.User).Include(x => x.SubscriptionType)
            .OrderByDescending(us => us.CreatedAt)
            .Take(count)
            .Select(us => new UserSubscription
            {
                Id = us.Id, UserId = us.UserId, UserName = us.User.FullName,
                SubscriptionTypeId = us.SubscriptionTypeId, SubscriptionTypeName = us.SubscriptionType.Name,
                Amount = us.Amount, StartDate = us.StartDate, EndDate = us.EndDate,
                Status = (SubscriptionStatus)(int)us.Status, IsPaid = us.IsPaid
            })
            .ToList();
    }
}
