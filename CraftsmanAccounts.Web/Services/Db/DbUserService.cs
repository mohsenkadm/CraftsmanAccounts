// خدمة المستخدمين المدعومة بقاعدة البيانات - البحث والتعديل والموافقة والرفض
using CraftsmanAccounts.Domain.Interfaces;
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Models.ViewModels;
using CraftsmanAccounts.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DomainUser = CraftsmanAccounts.Domain.Entities.AppUser;
using DomainApproval = CraftsmanAccounts.Domain.Enums.ApprovalStatus;

namespace CraftsmanAccounts.Web.Services.Db;

public class DbUserService : IUserService
{
    private readonly IUnitOfWork _uow;
    public DbUserService(IUnitOfWork uow) => _uow = uow;

    public UserFilterViewModel GetUsers(UserFilterViewModel filter)
    {
        var q = _uow.Repository<DomainUser>().Query().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            q = q.Where(u => u.FullName.Contains(filter.SearchTerm) || u.PhoneNumber.Contains(filter.SearchTerm));
        if (filter.IsActive.HasValue)
            q = q.Where(u => u.IsActive == filter.IsActive.Value);

        filter.TotalCount = q.Count();

        var sorted = filter.SortBy?.ToLower() switch
        {
            "name" => filter.SortDescending ? q.OrderByDescending(u => u.FullName) : q.OrderBy(u => u.FullName),
            "date" => filter.SortDescending ? q.OrderByDescending(u => u.CreatedAt) : q.OrderBy(u => u.CreatedAt),
            _ => q.OrderByDescending(u => u.CreatedAt)
        };

        filter.Users = sorted
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(u => ToWebUser(u))
            .ToList();

        return filter;
    }

    public List<AppUser> GetPendingUsers()
    {
        return _uow.Repository<DomainUser>().Query()
            .Where(u => u.ApprovalStatus == DomainApproval.Pending)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => ToWebUser(u))
            .ToList();
    }

    public AppUser? GetById(int id)
    {
        var u = _uow.Repository<DomainUser>().Query().FirstOrDefault(x => x.Id == id);
        return u == null ? null : ToWebUser(u);
    }

    public void Update(AppUser user)
    {
        var u = _uow.Repository<DomainUser>().Query().FirstOrDefault(x => x.Id == user.Id);
        if (u == null) return;
        u.FullName = user.FullName;
        u.Address = user.Address;
        u.PhoneNumber = user.PhoneNumber;
        u.IsActive = user.IsActive;
        _uow.Repository<DomainUser>().Update(u);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public void Delete(int id)
    {
        var u = _uow.Repository<DomainUser>().Query().FirstOrDefault(x => x.Id == id);
        if (u == null) return;
        _uow.Repository<DomainUser>().Remove(u);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public void Approve(int id)
    {
        var u = _uow.Repository<DomainUser>().Query().FirstOrDefault(x => x.Id == id);
        if (u == null) return;
        u.ApprovalStatus = DomainApproval.Approved;
        u.IsActive = true;
        _uow.Repository<DomainUser>().Update(u);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public void Reject(int id)
    {
        var u = _uow.Repository<DomainUser>().Query().FirstOrDefault(x => x.Id == id);
        if (u == null) return;
        u.ApprovalStatus = DomainApproval.Rejected;
        _uow.Repository<DomainUser>().Update(u);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public void ToggleActive(int id)
    {
        var u = _uow.Repository<DomainUser>().Query().FirstOrDefault(x => x.Id == id);
        if (u == null) return;
        u.IsActive = !u.IsActive;
        _uow.Repository<DomainUser>().Update(u);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public int GetTotalCount() => _uow.Repository<DomainUser>().Query().Count();
    public int GetActiveCount() => _uow.Repository<DomainUser>().Query().Count(u => u.IsActive);
    public int GetPendingCount() => _uow.Repository<DomainUser>().Query().Count(u => u.ApprovalStatus == DomainApproval.Pending);

    private static AppUser ToWebUser(DomainUser u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Address = u.Address,
        PhoneNumber = u.PhoneNumber,
        IsActive = u.IsActive,
        ApprovalStatus = (ApprovalStatus)(int)u.ApprovalStatus,
        CreatedAt = u.CreatedAt
    };
}
