// كيان المستخدم - يمثل مستخدم التطبيق مع بيانات التسجيل والتحقق
using CraftsmanAccounts.Domain.Enums;

namespace CraftsmanAccounts.Domain.Entities;

public class AppUser : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public string? OtpCode { get; set; }
    public DateTime? OtpExpiry { get; set; }
    public bool IsPhoneVerified { get; set; }
    public int OtpAttemptCount { get; set; }
    public DateTime? OtpLockedUntil { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Navigation
    public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
    public ICollection<Worker> Workers { get; set; } = new List<Worker>();
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    public ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
    public ICollection<ExpenseType> ExpenseTypes { get; set; } = new List<ExpenseType>();
    public ICollection<AccountEntry> AccountEntries { get; set; } = new List<AccountEntry>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
