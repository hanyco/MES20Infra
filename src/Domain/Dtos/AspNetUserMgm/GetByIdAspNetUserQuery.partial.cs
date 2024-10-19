using MediatR;
using Mes.HumanResources.Dtos;
using System;

namespace Mes.HumanResources.Dtos;
public sealed partial class GetByIdAspNetUserQuery : IRequest<GetByIdAspNetUserQueryResult>
{
    public GetByIdAspNetUserQuery()
    {
    }

    public String Id { get; set; }

    public GetByIdAspNetUserQuery(String id)
    {
        this.Id = id;
    }
}