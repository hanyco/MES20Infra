using Domain.Commands;
using Domain.Dtos;
using Domain.Queries;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator) => this._mediator = mediator;

    [HttpGet]
    public async Task<IEnumerable<Product>> GetAll()
    {
        var result = await this._mediator.Send(new GetAllProductsQuery());
        return result.Result;
    }

    [HttpGet("{id:long}")]
    public async Task<Product?> GetById(long id)
    {
        var result = await this._mediator.Send(new GetProductByIdQuery(id));
        return result.Result;
    }

    [HttpPost]
    public async Task<Result<long?>> Insert(Product product)
    {
        var result = await this._mediator.Send(new InsertProductCommand(product));
        return result;
    }

    [HttpPut]
    public async Task<Result> Update([FromBody]long id, Product product)
    {
        var result = await this._mediator.Send(new UpdateProductCommand(id, product));
        return result;
    }
    
    [HttpDelete("{id:long}")]
    public async Task<Result> Delete(long id)
    {
        var result = await this._mediator.Send(new DeleteProductCommand(id));
        return result;
    }

}