using Domain.Dtos;

using MediatR;

namespace Domain.Commands;

public sealed class InsertProductCommand : IRequest<InsertProductCommandResponse>
{
    public InsertProductCommand(ProductDto product) => this.Product = product;

    public ProductDto Product { get; }
}