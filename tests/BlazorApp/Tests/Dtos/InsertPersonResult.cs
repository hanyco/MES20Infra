using System;

namespace HumanResources.Dtos;
public sealed class InsertPersonResult
{
    public InsertPersonResult(Int64 id)
    {
        this.Id = id;
    }
    public Int64 Id { get; }
}