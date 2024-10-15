using System.Linq.Expressions;

namespace High.Processing.Core.Specs;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
}