using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using HumanResources.Queries;
using HumanResources.Dtos;
using Library.Results;

namespace HumanResources.Controllers;
[ApiControllerAttribute()]
[RouteAttribute("\"[controller]\"")]
public sealed class PersonController : ControllerBase
{
    private IMediator _mediator;
    public PersonController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [HttpGetAttribute()]
    public async Task<IEnumerable<PersonDto>> GetAll()
    {
        var result = await this._mediator.Send(new GetAllPeopleQuery());
        return result.Result;
    }

    [HttpGetAttribute("{id:long}")]
    public async Task<PersonDto> GetById(Int64 id)
    {
        var result = await this._mediator.Send(new GetByIdPersonQuery(id));
        return this.Ok(result);
    }

    [HttpPostAttribute()]
    public async Task<Result<long>> Insert(PersonDto personDto)
    {
        var result = await this._mediator.Send(new InsertPersonCommand(personDto));
        return result.Result;
    }

    [HttpPutAttribute("{id:long}")]
    public async Task<Result> Update(Int64 id, PersonDto personDto)
    {
        var result = await this._mediator.Send(new UpdatePersonCommand(id, personDto));
        return result.Result;
    }

    [HttpDeleteAttribute("{id:long}")]
    public async Task<Result> Delete(Int64 id)
    {
        var result = await this._mediator.Send(new DeletePersonCommand(id));
        return result;
    }
}