using High.Processing.Domain.Entity;

namespace High.Processing.Domain.Persistency;

public interface IUnitOfWork
{
    IRepository<Product> Products { get; }
    Task SaveAsync();
}