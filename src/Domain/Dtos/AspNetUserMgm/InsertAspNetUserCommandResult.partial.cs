using System;

namespace Mes.HumanResources.Dtos;
public sealed partial class InsertAspNetUserCommandResult
{
    public String Id { get; set; }

    public InsertAspNetUserCommandResult(String id)
    {
        this.Id = id;
    }
}