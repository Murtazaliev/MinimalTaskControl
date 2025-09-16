using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalTaskControl.Core.Entities;

namespace MinimalTaskControl.Infrastructure.Database.Configurations;

public class TaskRelationConfiguration : IEntityTypeConfiguration<TaskRelation>
{
    public void Configure(EntityTypeBuilder<TaskRelation> builder)
    {
        builder.HasKey(tr => new { tr.TaskId, tr.RelatedTaskId });

        builder.HasOne(tr => tr.Task)
            .WithMany(t => t.RelatedTasks)
            .HasForeignKey(tr => tr.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tr => tr.RelatedTask)
            .WithMany()
            .HasForeignKey(tr => tr.RelatedTaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
