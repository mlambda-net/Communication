using High.Processing.Core.Specs;
using MongoDB.Driver;

namespace High.Processing.Infrastructure.DataBase;

public static class SpecificationExtensions
{
    public static FilterDefinition<T> ToMongoFilter<T>(this ISpecification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        var expression = specification.ToExpression();
        return Builders<T>.Filter.Where(expression);
    }
}