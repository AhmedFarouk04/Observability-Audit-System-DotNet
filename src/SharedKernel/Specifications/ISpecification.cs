using System.Linq.Expressions;

namespace SharedKernel.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
}
