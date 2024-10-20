using MediatR;
using Mes.Infra.Security.Dtos;

namespace Mes.Infra.Security.Dtos;
public sealed partial class GetAllAspNetUsersQuery : IRequest<GetAllAspNetUsersQueryResult>
{
    public GetAllAspNetUsersQuery()
    {
    }
}