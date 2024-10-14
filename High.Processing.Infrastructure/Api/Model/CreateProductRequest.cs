namespace High.Processing.Infrastructure.Api.Model;

public record CreateProductRequest(
    string Code,
    string Name,
    string Description,
    float Price,
    string Category,
    string SubCategory,
    string Brand,
    float Size,
    float Weight
);