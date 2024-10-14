using High.Processing.Domain.Entity;
using High.Processing.Domain.Persistency;
using MongoDB.Driver;

namespace High.Processing.Infrastructure.DataBase;

public class UnitOfWork: IUnitOfWork
{
    private readonly IMongoDatabase _database;
    private  IRepository<Product> _products;

    public UnitOfWork(IMongoClient client, string database="default")
    {
        _database = client.GetDatabase(database);
        
    }
        
    
    public IRepository<Product> Products => _products ??= new MongoRepository<Product>(_database, "products");  
    
    
    public async Task SaveAsync()
    {
        await Task.CompletedTask;
    }
}