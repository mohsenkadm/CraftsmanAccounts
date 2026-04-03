// تكوين كيان المحفظة - قيود الأعمدة والعلاقة مع المستخدم
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class WalletConfig : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> b)
    {
        b.Property(w => w.Name).HasMaxLength(200).IsRequired();
        b.Property(w => w.Balance).HasColumnType("decimal(18,2)");
        b.HasOne(w => w.User).WithMany(u => u.Wallets).HasForeignKey(w => w.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
