using Domain.Dtos;

using MediatR;

namespace Domain.Commands;

public sealed class UpdateProductCommand : IRequest<UpdateProductCommandResponse>
{
    public UpdateProductCommand(ProductDto product) => this.Product = product;

    public ProductDto Product { get; }
}
