// خدمة المدير الوهمية - بيانات تجريبية للمديرين للاختبار بدون قاعدة بيانات
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Services.Mock;

public class MockAdminService : IAdminService
{
    private static readonly List<Admin> Admins =
    [
        new() { Id = 1, Username = "admin", Password = "admin123", DisplayName = "المدير العام" },
        new() { Id = 2, Username = "moderator", Password = "mod123", DisplayName = "المشرف" }
    ];

    public Admin? Authenticate(string username, string password)
    {
        return Admins.FirstOrDefault(a =>
            a.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            a.Password == password);
    }

    public List<Admin> GetAll() => Admins.ToList();

    public Admin? GetById(int id) => Admins.FirstOrDefault(a => a.Id == id);
}
