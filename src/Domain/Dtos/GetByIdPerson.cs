using System;

namespace HumanResource.Dtos;
public sealed class GetByIdPerson
{
    public Int64 Id { get; set; }

    public GetByIdPerson(Int64 id)
    {
        this.Id = id;
    }
}