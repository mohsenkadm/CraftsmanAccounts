// تكوين كيان اشتراك المستخدم - العلاقات مع المستخدم ونوع الاشتراك
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class UserSubscriptionConfig : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> b)
    {
        b.Property(s => s.Amount).HasColumnType("decimal(18,2)");
        b.HasOne(s => s.User).WithMany(u => u.Subscriptions).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(s => s.SubscriptionType).WithMany(t => t.UserSubscriptions).HasForeignKey(s => s.SubscriptionTypeId);
    }
}
