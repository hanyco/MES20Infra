using Domain.Dtos;

using Library.Results;

using MediatR;

namespace Domain.Commands;

public sealed class UpdateProductCommand : IRequest<Result>
{
    public UpdateProductCommand(Product product)
    {
        this.Product = product;
    }

    public Product Product { get; }
}
