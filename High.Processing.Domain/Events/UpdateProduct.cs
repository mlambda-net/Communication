namespace High.Processing.Domain.Events;

public record UpdateProduct(Guid Id, string Name, string Description);