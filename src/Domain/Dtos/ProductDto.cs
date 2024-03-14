namespace Domain.Dtos;

public sealed class ProductDto
{
    public string? Description { get; set; }

    public Guid Id { get; set; }

    public string? Name { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }
}