using Domain.Dtos;

using Library.Results;

using MediatR;

namespace Domain.Commands;

public sealed class InsertProductCommand : IRequest<Result<long?>>
{
    public InsertProductCommand(Product product)
    {
        this.Product = product;
    }

    public Product Product { get; }
}