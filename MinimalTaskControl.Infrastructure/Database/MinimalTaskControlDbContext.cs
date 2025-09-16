using Microsoft.EntityFrameworkCore;
using MinimalTaskControl.Core.Entities;

namespace MinimalTaskControl.Infrastructure.Database;

public class MinimalTaskControlDbContext : DbContext
{
    public MinimalTaskControlDbContext(DbContextOptions<MinimalTaskControlDbContext> options)
            : base(options)
    {
    }

    public MinimalTaskControlDbContext() : base()
    {
    }


    public DbSet<TaskInfo> Tasks => Set<TaskInfo>();
    public DbSet<TaskRelation> TaskRelations => Set<TaskRelation>();


    public const string DefaultSchema = "task_control_db";
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MinimalTaskControlDbContext).Assembly);
    }
}
