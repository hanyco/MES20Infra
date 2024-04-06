using Domain.Dtos;

using Library.Results;

using MediatR;

namespace Domain.Commands;

public sealed class UpdateProductCommand : IRequest<Result>
{
    public UpdateProductCommand(ProductDto product)
    {
        this.Product = product;
    }

    public ProductDto Product { get; }
}
