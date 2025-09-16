using System.Linq.Expressions;

namespace MinimalTaskControl.Core.Interfaces;

public interface ISpecificationFactory
{
    ISpecification<T> Create<T>(Expression<Func<T, bool>> criteria) where T : class;
}
