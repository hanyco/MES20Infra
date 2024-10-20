using MediatR;
using Mes.Infra.Auth.Dtos;

namespace Mes.Infra.Auth.Dtos;
public sealed partial class GetAllAspNetUsersQuery : IRequest<GetAllAspNetUsersQueryResult>
{
    public GetAllAspNetUsersQuery()
    {
    }
}