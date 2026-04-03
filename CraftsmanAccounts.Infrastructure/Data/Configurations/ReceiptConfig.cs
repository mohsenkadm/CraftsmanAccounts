// تكوين كيان سند القبض - العلاقات مع المستخدم والعميل والمشروع والمحفظة
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class ReceiptConfig : IEntityTypeConfiguration<Receipt>
{
    public void Configure(EntityTypeBuilder<Receipt> b)
    {
        b.Property(r => r.Amount).HasColumnType("decimal(18,2)");
        b.Property(r => r.Details).HasMaxLength(1000);
        b.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(r => r.Client).WithMany().HasForeignKey(r => r.ClientId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(r => r.Project).WithMany().HasForeignKey(r => r.ProjectId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(r => r.Wallet).WithMany().HasForeignKey(r => r.WalletId).OnDelete(DeleteBehavior.Restrict);
    }
}
