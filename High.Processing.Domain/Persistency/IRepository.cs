using High.Processing.Core.Specs;

namespace High.Processing.Domain.Persistency;

public interface IRepository<T>
{
    Task<T?> GetBy(ISpecification<T> spec);

    Task<IEnumerable<T>> Find(ISpecification<T> spec);

    Task Insert(T entity);

    Task Update(ISpecification<T> spec, T entity);

    Task Delete(ISpecification<T> spec);
}