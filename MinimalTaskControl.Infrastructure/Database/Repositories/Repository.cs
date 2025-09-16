using Microsoft.EntityFrameworkCore;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Core.Interfaces.Repositories;
using System.Linq.Expressions;

namespace MinimalTaskControl.Infrastructure.Database.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly MinimalTaskControlDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(MinimalTaskControlDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<TResult?> GetFirstOrDefaultAsync<TResult>(
        ISpecification<T> spec,
        CancellationToken cancellationToken)
        where TResult : class
    {
        var query = ApplySpecification(spec);

        if (spec.Select != null)
        {
            // Кастуем Expression<Func<T, object>> к Expression<Func<T, TResult>>
            var selectExpression = Expression.Lambda<Func<T, TResult>>(
            Expression.Convert(spec.Select.Body, typeof(TResult)),
            spec.Select.Parameters
            );
            return await query.Select(selectExpression).FirstOrDefaultAsync(cancellationToken);
        }

        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        return entity as TResult;
    }

    public virtual async Task<T?> GetFirstOrDefaultAsync(
        ISpecification<T> spec,
        CancellationToken cancellationToken)
    {
        var query = ApplySpecification(spec);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec).ToListAsync(cancellationToken);
    }

    public virtual async Task<List<TResult>?> ListAsync<TResult>(
     ISpecification<T> spec,
     CancellationToken cancellationToken)
     where TResult : class
    {
        var query = ApplySpecification(spec);

        if (spec.Select != null)
        {
            // Кастуем Expression<Func<T, object>> к Expression<Func<T, TResult>>
            var selectExpression = Expression.Lambda<Func<T, TResult>>(
                Expression.Convert(spec.Select.Body, typeof(TResult)),
                spec.Select.Parameters
            );

            return await query.Select(selectExpression).ToListAsync(cancellationToken);
        }

        var entities = await query.ToListAsync(cancellationToken);
        return entities.Cast<TResult>().ToList();
    }

    public virtual async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec, true).CountAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(spec, true).AnyAsync(cancellationToken);
    }

    protected virtual IQueryable<T> ApplySpecification(ISpecification<T> spec, bool forCount = false)
    {
        var query = _dbSet.AsQueryable();

        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        if (!forCount)
        {
            query = spec.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            query = spec.IncludeStrings
                .Aggregate(query, (current, include) => current.Include(include));

            if (spec.Skip.HasValue)
            {
                query = query.Skip(spec.Skip.Value);
            }
            if (spec.Take.HasValue)
            {
                query = query.Take(spec.Take.Value);
            }
        }

        if (spec.OrderBy != null)
        {
            query = query.OrderBy(spec.OrderBy);
        }

        else if (spec.OrderByDescending != null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }


        return query;
    }
}

