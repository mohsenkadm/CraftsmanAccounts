// نموذج المدير - بيانات مدير النظام في لوحة التحكم
namespace CraftsmanAccounts.Web.Models;

public class Admin
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
