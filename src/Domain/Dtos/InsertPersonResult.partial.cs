using System;

namespace HumanResources.Dtos;
public sealed partial class InsertPersonResult
{
    public Int64 Id { get; set; }

    public InsertPersonResult(Int64 id)
    {
        this.Id = id;
    }

    public InsertPersonResult()
    {
    }
}