using System.Linq.Expressions;

namespace MinimalTaskControl.Core.Interfaces;

public interface ISpecification<T>
{
    int? Skip { get; }
    int? Take { get; }

    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>>? Select { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }

    void ApplyPaging(int skip, int take);
    void AddInclude(Expression<Func<T, object>> includeExpression);
    void AddInclude(string includeString);
    void AddSelect(Expression<Func<T, object>> selectExpression);
    void AddOrderBy(Expression<Func<T, object>> orderByExpression);
    void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression);
}
