using System;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class UpdatePersonCommandResult
{
    public Int64 Id { get; set; }
    public PersonDto Person { get; set; }

    public UpdatePersonCommandResult(Int64 id, PersonDto person)
    {
        this.Id = id;
        this.Person = person;
    }
}