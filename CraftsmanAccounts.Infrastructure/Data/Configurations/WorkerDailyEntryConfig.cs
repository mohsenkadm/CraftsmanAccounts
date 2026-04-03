// تكوين كيان تفاصيل اليومية - العلاقات مع اليومية والعامل وسلوك الحذف
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class WorkerDailyEntryConfig : IEntityTypeConfiguration<WorkerDailyEntry>
{
    public void Configure(EntityTypeBuilder<WorkerDailyEntry> b)
    {
        b.Property(e => e.DailyRate).HasColumnType("decimal(18,2)");
        b.Property(e => e.ExtraAmount).HasColumnType("decimal(18,2)");
        b.HasOne(e => e.WorkerDaily).WithMany(d => d.Entries).HasForeignKey(e => e.WorkerDailyId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(e => e.Worker).WithMany(w => w.DailyEntries).HasForeignKey(e => e.WorkerId).OnDelete(DeleteBehavior.Restrict);
    }
}
