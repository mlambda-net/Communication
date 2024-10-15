namespace High.Processing.Infrastructure.Api.Model;

public record ProductResponse(
    Guid Id,
    string Code,
    string Name,
    string Description,
    string Category,
    string Brand,
    float Size,
    float Weight,
    float Price
);