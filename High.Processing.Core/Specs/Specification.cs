using System.Linq.Expressions;

namespace High.Processing.Core.Specs;

public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity)
    {
        return ToExpression().Compile().Invoke(entity);
    }

    public Specification<T> And(ISpecification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }

    public Specification<T> Or(ISpecification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

public class AndSpecification<T>(ISpecification<T> left, ISpecification<T> right) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExp = left.ToExpression();
        var rightExp = right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                Expression.Invoke(leftExp, parameter),
                Expression.Invoke(rightExp, parameter)),
            parameter);

        return combined;
    }
}

public class OrSpecification<T>(ISpecification<T> left, ISpecification<T> right) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExp = left.ToExpression();
        var rightExp = right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                Expression.Invoke(leftExp, parameter),
                Expression.Invoke(rightExp, parameter)),
            parameter);

        return combined;
    }
}

public class NotSpecification<T>(ISpecification<T> original) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var originalExp = original.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.Lambda<Func<T, bool>>(
            Expression.Not(Expression.Invoke(originalExp, parameter)),
            parameter);

        return combined;
    }
}