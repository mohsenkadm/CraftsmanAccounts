// تكوين كيان القيد المحاسبي - العلاقات مع المستخدم والعامل والعميل والمشروع والمحفظة
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class AccountEntryConfig : IEntityTypeConfiguration<AccountEntry>
{
    public void Configure(EntityTypeBuilder<AccountEntry> b)
    {
        b.Property(a => a.Amount).HasColumnType("decimal(18,2)");
        b.Property(a => a.Description).HasMaxLength(500);
        b.HasOne(a => a.User).WithMany(u => u.AccountEntries).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.Worker).WithMany(w => w.AccountEntries).HasForeignKey(a => a.WorkerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.Client).WithMany(c => c.AccountEntries).HasForeignKey(a => a.ClientId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.Project).WithMany(p => p.AccountEntries).HasForeignKey(a => a.ProjectId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.Wallet).WithMany().HasForeignKey(a => a.WalletId).OnDelete(DeleteBehavior.Restrict);
    }
}
