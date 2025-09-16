using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinimalTaskControl.Core.Entities;

namespace MinimalTaskControl.Infrastructure.Database.Configurations
{
    public class TaskInfoConfiguration : IEntityTypeConfiguration<TaskInfo>
    {
        public void Configure(EntityTypeBuilder<TaskInfo> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Author)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Assignee)
                .HasMaxLength(100);

            builder.Property(t => t.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.Priority)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.ParentTaskId)
                .IsRequired(false); 
            
            builder.HasOne(t => t.ParentTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.Navigation(t => t.SubTasks)
                .AutoInclude(false);

            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.Priority);
            builder.HasIndex(t => t.Author);
            builder.HasIndex(t => t.Assignee);
            builder.HasIndex(t => t.ParentTaskId);
        }
    }
}
