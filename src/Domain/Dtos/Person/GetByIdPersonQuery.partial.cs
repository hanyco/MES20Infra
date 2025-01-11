using MediatR;
using Mes.HumanResourcesManagement.Dtos;
using System;

namespace Mes.HumanResourcesManagement.Dtos;
public sealed partial class GetByIdPersonQuery : IRequest<GetByIdPersonQueryResult>
{
    public GetByIdPersonQuery()
    {
    }

    public Int64 Id { get; set; }

    public GetByIdPersonQuery(Int64 id)
    {
        this.Id = id;
    }
}