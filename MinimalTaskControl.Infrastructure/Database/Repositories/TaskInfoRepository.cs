using Microsoft.EntityFrameworkCore;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Interfaces.Repositories;

namespace MinimalTaskControl.Infrastructure.Database.Repositories;

public class TaskInfoRepository : ITaskInfoRepository
{
    private readonly MinimalTaskControlDbContext _context;
    private readonly DbSet<TaskInfo> _dbSet;

    public TaskInfoRepository(MinimalTaskControlDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TaskInfo>();
    }

    public async Task AddAsync(TaskInfo task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        await _dbSet.AddAsync(task, cancellationToken);
    }

    public async Task UpdateAsync(TaskInfo task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        var existingTask = await _dbSet
            .Include(t => t.SubTasks)
            .Include(t => t.RelatedTasks)
            .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken) ?? throw new InvalidOperationException($"Задача с ID {task.Id} не найдена");
        _context.Entry(existingTask).CurrentValues.SetValues(task);

        UpdateSubtasks(existingTask, task.SubTasks);

        UpdateRelatedTasks(existingTask, task.RelatedTasks);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(t => t.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    #region Private Methods

    private void UpdateSubtasks(TaskInfo existingTask, ICollection<TaskInfo> newSubtasks)
    {
        foreach (var existingSubtask in existingTask.SubTasks.ToList())
        {
            if (!newSubtasks.Any(s => s.Id == existingSubtask.Id))
                existingTask.SubTasks.Remove(existingSubtask);
        }

        foreach (var newSubtask in newSubtasks)
        {
            var existingSubtask = existingTask.SubTasks
                .FirstOrDefault(s => s.Id == newSubtask.Id);

            if (existingSubtask == null)
                existingTask.SubTasks.Add(newSubtask);
            else
                _context.Entry(existingSubtask).CurrentValues.SetValues(newSubtask);
        }
    }

    private void UpdateRelatedTasks(TaskInfo existingTask, ICollection<TaskRelation> newRelatedTasks)
    {
        foreach (var existingRelation in existingTask.RelatedTasks.ToList())
        {
            if (!newRelatedTasks.Any(r => r.Id == existingRelation.Id))
                _context.Remove(existingRelation);
        }

        foreach (var newRelation in newRelatedTasks)
        {
            var existingRelation = existingTask.RelatedTasks
                .FirstOrDefault(r => r.Id == newRelation.Id);

            if (existingRelation == null)
            {
                existingTask.RelatedTasks.Add(newRelation);
            }
            else
            {
                _context.Entry(existingRelation).CurrentValues.SetValues(newRelation);
            }
        }
    }

    #endregion
}
