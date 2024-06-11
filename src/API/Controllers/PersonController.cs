using Microsoft.AspNetCore.Mvc;
using MediatR;
using Library.Types;

namespace HumanResource.Controllers;
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
    public Task<IEnumerable<PersonDto>> GetAll()
    {
        var result = await this._mediator.Send(new GetAllPeopleQuery());
        return result.Result;
    }

    [HttpGetAttribute("{id:long}")]
    public Task<PersonDto>? GetById()
    {
        var result = await this._mediator.Send(new GetByIdPersonQuery(id));
        return result.Result;
    }

    [HttpPostAttribute()]
    public Task<Result<Int64>, CoreLib,  0 , Culture =  neutral, PublicKeyToken =  7  cec85d7bea7798e ] ]> Insert()
    {
        var result = await this._mediator.Send(new InsertPersonCommand(personDto));
        return result.Result;
    }

    [HttpPutAttribute("{id:long}")]
    public Task<Result> Update()
    {
        var result = await this._mediator.Send(new UpdatePersonCommand(id, personDto));
        return result.Result;
    }

    [HttpDeleteAttribute("{id:long}")]
    public Task<Result> Delete()
    {
        var result = await this._mediator.Send(new DeletePersonCommand(id));
        return result;
    }
}