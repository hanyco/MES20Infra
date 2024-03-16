using Domain.Dtos;
using Domain.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator) => this._mediator = mediator;

    [HttpGet]
    public async Task<IEnumerable<ProductDto>> GetAll()
    {
        var result = await this._mediator.Send(new GetAllProductsQuery());
        return result.Products;
    }

    [HttpGet("{id:long}")]
    public async Task<ProductDto?> GetById(long id)
    {
        var result = await this._mediator.Send(new GetProductByIdQuery(id));
        return result.Product;
    }
}