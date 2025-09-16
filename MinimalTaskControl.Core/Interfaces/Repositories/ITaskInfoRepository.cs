using MinimalTaskControl.Core.Entities;

namespace MinimalTaskControl.Core.Interfaces.Repositories
{
    public interface ITaskInfoRepository
    {
        Task AddAsync(TaskInfo task, CancellationToken cancellationToken = default);
        Task UpdateAsync(TaskInfo task, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
