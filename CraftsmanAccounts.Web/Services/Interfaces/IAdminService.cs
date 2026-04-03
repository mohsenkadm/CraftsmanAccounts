// واجهة خدمة المدير - تعريف عمليات المصادقة وإدارة المديرين
using CraftsmanAccounts.Web.Models;

namespace CraftsmanAccounts.Web.Services.Interfaces;

/// <summary>
/// واجهة خدمة المدير للمصادقة واسترجاع بيانات المديرين
/// </summary>
public interface IAdminService
{
    Admin? Authenticate(string username, string password);
    List<Admin> GetAll();
    Admin? GetById(int id);
}
