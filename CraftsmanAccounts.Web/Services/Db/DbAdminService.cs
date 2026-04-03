// خدمة المدير المدعومة بقاعدة البيانات - مصادقة المدير واسترجاع بياناته
using System.Security.Cryptography;
using System.Text;
using CraftsmanAccounts.Domain.Interfaces;
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using DomainAdmin = CraftsmanAccounts.Domain.Entities.Admin;

namespace CraftsmanAccounts.Web.Services.Db;

public class DbAdminService : IAdminService
{
    private readonly IUnitOfWork _uow;
    public DbAdminService(IUnitOfWork uow) => _uow = uow;

    public Admin? Authenticate(string username, string password)
    {
        var hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        var admin = _uow.Repository<DomainAdmin>().Query()
            .FirstOrDefault(a => a.Username == username && a.PasswordHash == hash);
        return admin == null ? null : new Admin { Id = admin.Id, Username = admin.Username, DisplayName = admin.DisplayName };
    }

    public List<Admin> GetAll()
    {
        return _uow.Repository<DomainAdmin>().Query()
            .Select(a => new Admin { Id = a.Id, Username = a.Username, DisplayName = a.DisplayName })
            .ToList();
    }

    public Admin? GetById(int id)
    {
        var a = _uow.Repository<DomainAdmin>().Query().FirstOrDefault(x => x.Id == id);
        return a == null ? null : new Admin { Id = a.Id, Username = a.Username, DisplayName = a.DisplayName };
    }
}
