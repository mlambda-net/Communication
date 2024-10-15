using High.Processing.Domain.Events;

namespace High.Processing.App;

public class LogHandler
{
    public Task Create(CreateProduct msg)
    {
        Console.WriteLine($"receiving create product id: {msg.Product.Id}");
        return Task.CompletedTask;
    }


    public Task Delete(DeleteProduct msg)
    {
        Console.WriteLine($"receiving delete product id: {msg.Id}");
        return Task.CompletedTask;
    }


    public Task Update(UpdateProduct msg)
    {
        Console.WriteLine($"receiving update product id: {msg.Id}");
        return Task.CompletedTask;
    }
}