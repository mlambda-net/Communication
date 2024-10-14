using High.Processing.Domain.Entity;
using High.Processing.Domain.Events;
using High.Processing.Domain.Persistency;
using High.Processing.Domain.Query;

namespace High.Processing.App;

public class PersistentHandler(IUnitOfWork uow)
{
    public async Task Create(CreateProduct msg)
    {
        await uow.Products.Insert(msg.Product);
    }


    public async Task Delete(DeleteProduct msg)
    {
        await uow.Products.Delete(new ProductById(msg.Id));
    }


    public async Task Update(UpdateProduct msg)
    {
        var byId = new ProductById(msg.Id);
        var product = await uow.Products.GetBy(byId);
        if (product != null)
        {
            product.Name = msg.Name;
            product.Description = msg.Description;
            await uow.Products.Update(byId, product);
        }
    }


}