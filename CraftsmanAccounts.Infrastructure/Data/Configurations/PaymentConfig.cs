// تكوين كيان سند الصرف - العلاقات مع المستخدم ونوع المصروف والمشروع والعميل والعامل والمحفظة
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class PaymentConfig : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        b.Property(p => p.Details).HasMaxLength(1000);
        b.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.ExpenseType).WithMany(e => e.Payments).HasForeignKey(p => p.ExpenseTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Project).WithMany().HasForeignKey(p => p.ProjectId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Client).WithMany().HasForeignKey(p => p.ClientId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Worker).WithMany().HasForeignKey(p => p.WorkerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Wallet).WithMany().HasForeignKey(p => p.WalletId).OnDelete(DeleteBehavior.Restrict);
    }
}
