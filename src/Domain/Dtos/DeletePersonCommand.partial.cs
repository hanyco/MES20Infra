using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;
using System;

namespace HumanResources.Dtos;
public sealed partial class DeletePersonCommand : IQuery<DeletePersonCommandResult>
{
    public Int64 Id { get; set; }

    public DeletePersonCommand(Int64 id)
    {
        this.Id = id;
    }
}