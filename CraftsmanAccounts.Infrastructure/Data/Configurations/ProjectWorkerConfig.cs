// تكوين كيان عامل المشروع - فهرس فريد مركب والعلاقات مع المشروع والعامل
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CraftsmanAccounts.Domain.Entities;

namespace CraftsmanAccounts.Infrastructure.Data.Configurations;

public class ProjectWorkerConfig : IEntityTypeConfiguration<ProjectWorker>
{
    public void Configure(EntityTypeBuilder<ProjectWorker> b)
    {
        b.HasIndex(pw => new { pw.ProjectId, pw.WorkerId }).IsUnique();
        b.Property(pw => pw.DailyRate).HasColumnType("decimal(18,2)");
        b.HasOne(pw => pw.Project).WithMany(p => p.ProjectWorkers).HasForeignKey(pw => pw.ProjectId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(pw => pw.Worker).WithMany(w => w.ProjectWorkers).HasForeignKey(pw => pw.WorkerId).OnDelete(DeleteBehavior.Restrict);
    }
}
