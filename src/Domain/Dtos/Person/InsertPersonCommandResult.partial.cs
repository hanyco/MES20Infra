using System;

namespace Mes.HumanResourcesManagement.Dtos;
public sealed partial class InsertPersonCommandResult
{
    public Int64 Id { get; set; }

    public InsertPersonCommandResult(Int64 id)
    {
        this.Id = id;
    }
}