using System;

namespace Mes.Infra.Auth.Dtos;
public sealed partial class InsertAspNetUserCommandResult
{
    public String Id { get; set; }

    public InsertAspNetUserCommandResult(String id)
    {
        this.Id = id;
    }
}