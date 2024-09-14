using System;

namespace HumanResource.Dtos;
public sealed class InsertPersonResult
{
    public Int64 Id { get; set; }

    public InsertPersonResult(Int64 id)
    {
        this.Id = id;
    }
}