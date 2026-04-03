// واجهة خدمة المستخدمين - تعريف عمليات إدارة المستخدمين والموافقات
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Models.ViewModels;

namespace CraftsmanAccounts.Web.Services.Interfaces;

/// <summary>
/// واجهة خدمة المستخدمين للبحث والتعديل والموافقة والرفض
/// </summary>
public interface IUserService
{
    UserFilterViewModel GetUsers(UserFilterViewModel filter);
    List<AppUser> GetPendingUsers();
    AppUser? GetById(int id);
    void Update(AppUser user);
    void Delete(int id);
    void Approve(int id);
    void Reject(int id);
    void ToggleActive(int id);
    int GetTotalCount();
    int GetActiveCount();
    int GetPendingCount();
}
