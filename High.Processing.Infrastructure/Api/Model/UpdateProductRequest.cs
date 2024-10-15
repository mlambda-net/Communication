namespace High.Processing.Infrastructure.Api.Model;

public record UpdateProductRequest(Guid Id, string Name, string Description);