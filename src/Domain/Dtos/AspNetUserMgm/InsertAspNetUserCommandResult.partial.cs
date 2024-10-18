using System;

namespace Mes.Security.Dtos;
public sealed partial class InsertAspNetUserCommandResult
{
    public String Id { get; set; }

    public InsertAspNetUserCommandResult(String id)
    {
        this.Id = id;
    }
}