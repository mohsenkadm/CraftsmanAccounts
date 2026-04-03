// تكوين كيان العميل - قيود الأعمدة والعلاقة مع المستخدم
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class ClientConfig : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> b)
    {
        b.Property(c => c.Name).HasMaxLength(200).IsRequired();
        b.Property(c => c.Address).HasMaxLength(500);
        b.Property(c => c.PhoneNumber).HasMaxLength(20);
        b.HasOne(c => c.User).WithMany(u => u.Clients).HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
