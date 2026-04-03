// تكوين كيان المدير - الفهارس وقيود الأعمدة
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class AdminConfig : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> b)
    {
        b.HasIndex(a => a.Username).IsUnique();
        b.Property(a => a.Username).HasMaxLength(100).IsRequired();
        b.Property(a => a.PasswordHash).HasMaxLength(256).IsRequired();
        b.Property(a => a.DisplayName).HasMaxLength(200).IsRequired();
    }
}
