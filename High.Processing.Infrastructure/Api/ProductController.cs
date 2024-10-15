using High.Processing.Domain.Entity;
using High.Processing.Domain.Events;
using High.Processing.Domain.Persistency;
using High.Processing.Domain.Query;
using High.Processing.Domain.Services;
using High.Processing.Infrastructure.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace High.Processing.Infrastructure.Api;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IUnitOfWork uow, IEventSender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Get(Guid id)
    {
        var item = await uow.Products.GetBy(ProductById.ById(id));
        if (item == null) return NotFound();

        return Ok(new ProductResponse(
            item.Id,
            item.Code,
            item.Name,
            item.Description,
            item.Category,
            item.Brand,
            item.Size,
            item.Weight,
            item.Price));
    }

    [HttpPost]
    public async Task<ActionResult<CreateProductResponse>> Post(CreateProductRequest item)
    {
        var product = new Product
        {
            Brand = item.Brand,
            Description = item.Description,
            Category = item.Category,
            SubCategory = item.SubCategory,
            Code = item.Code,
            Name = item.Name,
            Price = item.Price,
            Size = item.Size,
            Weight = item.Weight
        };
        await sender.Send(new CreateProduct(product));
        return Ok(new CreateProductResponse(product.Id));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, UpdateProductRequest item)
    {
        await sender.Send(new UpdateProduct(id, item.Name, item.Description));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await sender.Send(new DeleteProduct(id));
        return NoContent();
    }
}