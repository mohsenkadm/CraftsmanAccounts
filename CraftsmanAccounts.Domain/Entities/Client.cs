// كيان العميل - يمثل عميل (مهندس أو مقاول أو صاحب منزل)
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Domain.Entities;

public class Client : UserOwnedEntity
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public ClientType ClientType { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<AccountEntry> AccountEntries { get; set; } = new List<AccountEntry>();
}
