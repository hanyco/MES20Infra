using System;

namespace HumanResources.Dtos;
public sealed partial class UpdatePersonResult
{
    public Int64 Id { get; set; }

    public UpdatePersonResult(Int64 id)
    {
        this.Id = id;
    }

    public UpdatePersonResult()
    {
    }
}