// خدمة أنواع الاشتراكات المدعومة بقاعدة البيانات - إدارة أنواع الاشتراكات بالكامل
using CraftsmanAccounts.Domain.Interfaces;
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DomainST = CraftsmanAccounts.Domain.Entities.SubscriptionType;

namespace CraftsmanAccounts.Web.Services.Db;

public class DbSubscriptionTypeService : ISubscriptionTypeService
{
    private readonly IUnitOfWork _uow;
    public DbSubscriptionTypeService(IUnitOfWork uow) => _uow = uow;

    public List<SubscriptionType> GetAll()
    {
        return _uow.Repository<DomainST>().Query()
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SubscriptionType
            {
                Id = s.Id, Name = s.Name, Amount = s.Amount,
                DurationInDays = s.DurationInDays, Details = s.Details,
                IsActive = s.IsActive, CreatedAt = s.CreatedAt
            })
            .ToList();
    }

    public SubscriptionType? GetById(int id)
    {
        var s = _uow.Repository<DomainST>().Query().FirstOrDefault(x => x.Id == id);
        return s == null ? null : new SubscriptionType
        {
            Id = s.Id, Name = s.Name, Amount = s.Amount,
            DurationInDays = s.DurationInDays, Details = s.Details,
            IsActive = s.IsActive, CreatedAt = s.CreatedAt
        };
    }

    public void Create(SubscriptionType model)
    {
        var entity = new DomainST { Name = model.Name, Amount = model.Amount, DurationInDays = model.DurationInDays, Details = model.Details, IsActive = model.IsActive };
        _uow.Repository<DomainST>().AddAsync(entity).GetAwaiter().GetResult();
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public void Update(SubscriptionType model)
    {
        var s = _uow.Repository<DomainST>().Query().FirstOrDefault(x => x.Id == model.Id);
        if (s == null) return;
        s.Name = model.Name; s.Amount = model.Amount; s.DurationInDays = model.DurationInDays; s.Details = model.Details; s.IsActive = model.IsActive;
        _uow.Repository<DomainST>().Update(s);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public void Delete(int id)
    {
        var s = _uow.Repository<DomainST>().Query().FirstOrDefault(x => x.Id == id);
        if (s == null) return;
        _uow.Repository<DomainST>().Remove(s);
        _uow.SaveChangesAsync().GetAwaiter().GetResult();
    }
}
