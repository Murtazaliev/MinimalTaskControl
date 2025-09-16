using MinimalTaskControl.Core.Entities;

namespace MinimalTaskControl.Core.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<TResult?> GetFirstOrDefaultAsync<TResult>(ISpecification<T> spec, CancellationToken cancellationToken) where TResult : class;
    Task<T?> GetFirstOrDefaultAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<List<TResult>?> ListAsync<TResult>(ISpecification<T> spec, CancellationToken cancellationToken) where TResult : class;
    Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken cancellationToken = default);
}
