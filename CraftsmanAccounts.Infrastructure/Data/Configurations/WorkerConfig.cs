// تكوين كيان العامل - قيود الأعمدة والعلاقة مع المستخدم
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class WorkerConfig : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> b)
    {
        b.Property(w => w.Name).HasMaxLength(200).IsRequired();
        b.Property(w => w.Address).HasMaxLength(500);
        b.Property(w => w.PhoneNumber).HasMaxLength(20);
        b.HasOne(w => w.User).WithMany(u => u.Workers).HasForeignKey(w => w.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
