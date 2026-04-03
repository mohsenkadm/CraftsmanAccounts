// تكوين كيان المشروع - قيود الأعمدة والعلاقات مع المستخدم والعميل
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class ProjectConfig : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> b)
    {
        b.Property(p => p.Name).HasMaxLength(300).IsRequired();
        b.Property(p => p.Address).HasMaxLength(500);
        b.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");
        b.Property(p => p.PaidAmount).HasColumnType("decimal(18,2)");
        b.HasOne(p => p.User).WithMany(u => u.Projects).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(p => p.Client).WithMany(c => c.Projects).HasForeignKey(p => p.ClientId).OnDelete(DeleteBehavior.Restrict);
    }
}
