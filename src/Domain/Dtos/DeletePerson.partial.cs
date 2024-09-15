using System;

namespace HumanResources.Dtos;
public sealed partial class DeletePerson
{
    public Int64 Id { get; set; }

    public DeletePerson(Int64 id)
    {
        this.Id = id;
    }

    public DeletePerson()
    {
    }
}