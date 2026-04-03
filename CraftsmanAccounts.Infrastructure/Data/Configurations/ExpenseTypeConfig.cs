// تكوين كيان نوع المصروف - قيود الأعمدة والعلاقة مع المستخدم
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class ExpenseTypeConfig : IEntityTypeConfiguration<ExpenseType>
{
    public void Configure(EntityTypeBuilder<ExpenseType> b)
    {
        b.Property(e => e.Name).HasMaxLength(200).IsRequired();
        b.HasOne(e => e.User).WithMany(u => u.ExpenseTypes).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
