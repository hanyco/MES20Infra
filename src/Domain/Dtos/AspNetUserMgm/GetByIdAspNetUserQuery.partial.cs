using MediatR;
using Mes.Infra.Security.Dtos;
using System;

namespace Mes.Infra.Security.Dtos;
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