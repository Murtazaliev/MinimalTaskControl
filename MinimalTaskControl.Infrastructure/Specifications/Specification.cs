using MinimalTaskControl.Core.Interfaces;
using System.Linq.Expressions;

namespace MinimalTaskControl.Infrastructure.Specifications
{
    public class Specification<T> : ISpecification<T> where T : class
    {
        public Specification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        public int? Skip { get; private set; }
        public int? Take { get; private set; }

        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>>? Select { get; private set; }
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }


        public void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        public void AddInclude(Expression<Func<T, object>> includeExpression)
            => Includes.Add(includeExpression);

        public void AddInclude(string includeString)        
            => IncludeStrings.Add(includeString);        

        public void AddSelect(Expression<Func<T, object>> selectExpression)
            => Select = selectExpression;
        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        => OrderBy = orderByExpression;

        public void AddOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
            => OrderByDescending = orderByDescendingExpression;
    }
}
