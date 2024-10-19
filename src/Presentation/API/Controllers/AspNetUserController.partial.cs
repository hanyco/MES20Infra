using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using Mes.HumanResources.Dtos;
using Library.Results;

namespace Mes.HumanResources.Controllers;
[ApiControllerAttribute()]
[RouteAttribute("[controller]")]
public sealed class AspNetUserController : ControllerBase
{
    private IMediator _mediator;
    public AspNetUserController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [HttpGetAttribute()]
    public async Task<IActionResult> GetAll()
    {
        var result = await this._mediator.Send(new GetAllAspNetUsersQuery());
        return this.Ok(result.AspNetUsers);
    }

    [HttpGetAttribute("{id}")]
    public async Task<IActionResult> GetById(String id)
    {
        var result = await this._mediator.Send(new GetByIdAspNetUserQuery(id));
        return this.Ok(result.AspNetUserDto);
    }

    [HttpPostAttribute()]
    public async Task<IActionResult> Insert(AspNetUserDto aspNetUserDto)
    {
        var result = await this._mediator.Send(new InsertAspNetUserCommand(aspNetUserDto));
        return this.Ok(result.Id);
    }

    [HttpPutAttribute("{id}")]
    public async Task<IActionResult> Update(String id, AspNetUserDto aspNetUserDto)
    {
        var result = await this._mediator.Send(new UpdateAspNetUserCommand(id, aspNetUserDto));
        return this.Ok(result);
    }

    [HttpDeleteAttribute("{id}")]
    public async Task<IActionResult> Delete(String id)
    {
        var result = await this._mediator.Send(new DeleteAspNetUserCommand(id));
        return this.Ok(true);
    }
}