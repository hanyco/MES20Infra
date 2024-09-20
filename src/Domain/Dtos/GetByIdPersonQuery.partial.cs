using MediatR;
using HumanResources.Dtos;
using System;

namespace HumanResources.Dtos;
public sealed partial class GetByIdPersonQuery : IRequest<GetByIdPersonQueryResult>
{
    public Int64 Id { get; set; }

    public GetByIdPersonQuery(Int64 id)
    {
        this.Id = id;
    }
}