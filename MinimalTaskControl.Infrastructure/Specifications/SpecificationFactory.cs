using MinimalTaskControl.Core.Interfaces;
using System.Linq.Expressions;

namespace MinimalTaskControl.Infrastructure.Specifications;

public class SpecificationFactory : ISpecificationFactory
{
    public ISpecification<T> Create<T>(Expression<Func<T, bool>> criteria) where T : class
    {
        return new Specification<T>(criteria);
    }
}