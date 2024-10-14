namespace High.Processing.Domain.Entity;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Brand { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }

    public string SubCategory { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public float Size { get; set; }
    public float Weight { get; set; }

    public float Tax { get; set; }
}