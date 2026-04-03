// سياق قاعدة البيانات - يحتوي على جميع جداول الكيانات ويطبق إعدادات التكوين
using Microsoft.EntityFrameworkCore;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<SubscriptionType> SubscriptionTypes => Set<SubscriptionType>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectWorker> ProjectWorkers => Set<ProjectWorker>();
    public DbSet<ProjectEquipment> ProjectEquipments => Set<ProjectEquipment>();
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ExpenseType> ExpenseTypes => Set<ExpenseType>();
    public DbSet<WorkerDaily> WorkerDailies => Set<WorkerDaily>();
    public DbSet<WorkerDailyEntry> WorkerDailyEntries => Set<WorkerDailyEntry>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<AccountEntry> AccountEntries => Set<AccountEntry>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
