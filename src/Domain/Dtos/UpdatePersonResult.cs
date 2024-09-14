using System;

namespace HumanResource.Dtos;
public sealed class UpdatePersonResult
{
    public Int64 Id { get; set; }

    public UpdatePersonResult(Int64 id)
    {
        this.Id = id;
    }
}