using System;

namespace HumanResources.Dtos;
public sealed partial class GetByIdPerson
{
    public Int64 Id { get; set; }

    public GetByIdPerson(Int64 id)
    {
        this.Id = id;
    }

    public GetByIdPerson()
    {
    }
}