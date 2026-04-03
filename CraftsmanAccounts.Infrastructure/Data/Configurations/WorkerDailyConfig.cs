// تكوين كيان اليومية - العلاقات مع المستخدم والمشروع والمحفظة
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class WorkerDailyConfig : IEntityTypeConfiguration<WorkerDaily>
{
    public void Configure(EntityTypeBuilder<WorkerDaily> b)
    {
        b.HasOne(d => d.User).WithMany().HasForeignKey(d => d.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(d => d.Project).WithMany(p => p.WorkerDailies).HasForeignKey(d => d.ProjectId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(d => d.Wallet).WithMany().HasForeignKey(d => d.WalletId).OnDelete(DeleteBehavior.Restrict);
    }
}
