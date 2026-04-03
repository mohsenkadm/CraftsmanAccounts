// تكوين كيان المستخدم - فهرس رقم الهاتف الفريد وقيود الأعمدة
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> b)
    {
        b.HasIndex(u => u.PhoneNumber).IsUnique();
        b.Property(u => u.FullName).HasMaxLength(200).IsRequired();
        b.Property(u => u.Address).HasMaxLength(500);
        b.Property(u => u.PhoneNumber).HasMaxLength(20).IsRequired();
        b.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        b.Property(u => u.OtpCode).HasMaxLength(10);
    }
}
