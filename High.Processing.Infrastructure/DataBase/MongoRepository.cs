using High.Processing.Core.Specs;
using High.Processing.Domain.Persistency;
using MongoDB.Driver;

namespace High.Processing.Infrastructure.DataBase;

public class MongoRepository<T>(IMongoDatabase database, string collectionName) : IRepository<T>
{
    private readonly IMongoCollection<T> _collection = database.GetCollection<T>(collectionName);

    public async Task Insert(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task Update(ISpecification<T> spec, T entity)
    {
        await _collection.ReplaceOneAsync(spec.ToMongoFilter(), entity);
    }

    public async Task Delete(ISpecification<T> spec)
    {
        await _collection.DeleteOneAsync(spec.ToExpression());
    }

    public async Task<T> GetBy(ISpecification<T> spec)
    {
        return await _collection.Find(spec.ToMongoFilter()).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> Find(ISpecification<T> spec)
    {
        return await _collection.Find(spec.ToMongoFilter()).ToListAsync();
    }
}