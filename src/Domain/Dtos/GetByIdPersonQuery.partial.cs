using MediatR;
using Mes.HumanResources.Dtos;
using System;

namespace Mes.HumanResources.Dtos;
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