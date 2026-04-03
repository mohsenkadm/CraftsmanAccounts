// كيان المدير - يمثل مدير لوحة التحكم
namespace CraftsmanAccounts.Domain.Entities;

public class Admin : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
