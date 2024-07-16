using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using HumanResources.Dtos;
using Library.Types;

namespace HumanResource.Controllers;

class PersonDto;

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
    public async Task<PersonDto> GetById()
    {
        var result = await this._mediator.Send(new GetByIdPersonQuery(id));
        return result.Result;
    }

    [HttpPostAttribute()]
    public async Task<Result<long>> Insert()
    {
        var result = await this._mediator.Send(new InsertPersonCommand(personDto));
        return result.Result;
    }

    [HttpPutAttribute("{id:long}")]
    public async Task<Result> Update()
    {
        var result = await this._mediator.Send(new UpdatePersonCommand(id, personDto));
        return result.Result;
    }

    [HttpDeleteAttribute("{id:long}")]
    public async Task<Result> Delete()
    {
        var result = await this._mediator.Send(new DeletePersonCommand(id));
        return result;
    }
}