using System;

namespace HumanResources.Dtos;
public sealed partial class UpdatePersonCommandResult
{
    public Int64 Id { get; set; }

    public UpdatePersonCommandResult(Int64 id)
    {
        this.Id = id;
    }
}