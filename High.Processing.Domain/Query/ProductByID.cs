using System.Linq.Expressions;
using High.Processing.Core.Specs;
using High.Processing.Domain.Entity;

namespace High.Processing.Domain.Query;

public class ProductById(Guid id) : ISpecification<Product>
{
    public Expression<Func<Product, bool>> ToExpression()
    {
        return file => file.Id == id;
    }

    public static ISpecification<Product> ById(Guid id)
    {
        return new ProductById(id);
    }
}