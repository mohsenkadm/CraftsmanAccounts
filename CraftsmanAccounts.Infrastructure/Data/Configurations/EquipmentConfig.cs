// تكوين كيان المعدات - قيود الأعمدة والعلاقة مع المستخدم
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class EquipmentConfig : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> b)
    {
        b.Property(e => e.Name).HasMaxLength(300).IsRequired();
        b.Property(e => e.PurchasedFrom).HasMaxLength(300);
        b.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        b.HasOne(e => e.User).WithMany(u => u.Equipment).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
