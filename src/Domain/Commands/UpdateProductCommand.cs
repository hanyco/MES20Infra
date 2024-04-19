using Domain.Dtos;

using Library.Results;

using MediatR;

namespace Domain.Commands;

public sealed class UpdateProductCommand : IRequest<Result>
{
    public UpdateProductCommand(long id, Product product)
    {
        this.Id = id;
        this.Product = product;
    }

    public long Id { get; }
    public Product Product { get; }
}