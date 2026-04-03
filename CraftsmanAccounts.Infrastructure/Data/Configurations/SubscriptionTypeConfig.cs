// تكوين كيان نوع الاشتراك - قيود الأعمدة ونوع العمود العشري
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class SubscriptionTypeConfig : IEntityTypeConfiguration<SubscriptionType>
{
    public void Configure(EntityTypeBuilder<SubscriptionType> b)
    {
        b.Property(s => s.Name).HasMaxLength(200).IsRequired();
        b.Property(s => s.Amount).HasColumnType("decimal(18,2)");
        b.Property(s => s.Details).HasMaxLength(1000);
    }
}
