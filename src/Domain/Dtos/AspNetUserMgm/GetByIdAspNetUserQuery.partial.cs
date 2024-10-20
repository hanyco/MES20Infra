using MediatR;
using Mes.System.Security.Dtos;
using System;

namespace Mes.System.Security.Dtos;
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