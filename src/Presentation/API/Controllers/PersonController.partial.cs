using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using Mes.HumanResourcesManagement.Dtos;
using Library.Results;
using Microsoft.AspNetCore.Authorization;

namespace Mes.HumanResourcesManagement.Controllers;
[ApiControllerAttribute()]
[RouteAttribute("[controller]")]
[AuthorizeAttribute]
public sealed class PersonController : ControllerBase
{
    private IMediator _mediator;
    public PersonController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [HttpGetAttribute()]
    public async Task<IActionResult> GetAll()
    {
        var result = await this._mediator.Send(new GetAllPeopleQuery());
        return this.Ok(result.People);
    }

    [HttpGetAttribute("{id}")]
    public async Task<IActionResult> GetById(Int64 id)
    {
        var result = await this._mediator.Send(new GetByIdPersonQuery(id));
        return this.Ok(result.PersonDto);
    }

    [HttpPostAttribute()]
    public async Task<IActionResult> Insert(PersonDto personDto)
    {
        var result = await this._mediator.Send(new InsertPersonCommand(personDto));
        return this.Ok(result.Id);
    }

    [HttpPutAttribute("{id}")]
    public async Task<IActionResult> Update(Int64 id, PersonDto personDto)
    {
        var result = await this._mediator.Send(new UpdatePersonCommand(id, personDto));
        return this.Ok(result);
    }

    [HttpDeleteAttribute("{id}")]
    public async Task<IActionResult> Delete(Int64 id)
    {
        var result = await this._mediator.Send(new DeletePersonCommand(id));
        return this.Ok(true);
    }
}