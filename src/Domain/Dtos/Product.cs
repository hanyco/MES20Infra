namespace Domain.Dtos;

public sealed class Product
{
    public string? Description { get; set; }

    public long Id { get; set; }

    public string? Name { get; set; }

    public decimal Price { get; set; }
}