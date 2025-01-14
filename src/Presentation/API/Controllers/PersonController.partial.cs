using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using Mes.HumanResourcesManagement.Dtos;
using Library.Results;
using Microsoft.AspNetCore.Authorization;
using HanyCo.Infra.Security;

namespace Mes.HumanResourcesManagement.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class PersonController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    [HttpGet]
    [Permission("Person:ViewAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await this._mediator.Send(new GetAllPeopleQuery());
        return this.Ok(result.People);
    }

    [HttpGet("{id}")]
    [Permission("Person:View")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await this._mediator.Send(new GetByIdPersonQuery(id));
        return this.Ok(result.PersonDto);
    }

    [HttpPost]
    [Permission("Person:Create")]
    public async Task<IActionResult> Insert(PersonDto personDto)
    {
        var result = await this._mediator.Send(new InsertPersonCommand(personDto));
        return this.Ok(result.Id);
    }

    [HttpPut("{id}")]
    [Permission("Person:Update")]
    public async Task<IActionResult> Update(long id, PersonDto personDto)
    {
        var result = await this._mediator.Send(new UpdatePersonCommand(id, personDto));
        return this.Ok(result);
    }

    [HttpDelete("{id}")]
    [Permission("Person:Delete")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await this._mediator.Send(new DeletePersonCommand(id));
        return this.Ok(true);
    }
}
