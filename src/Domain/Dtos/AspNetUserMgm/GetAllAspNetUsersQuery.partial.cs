using MediatR;
using Mes.System.Security.Dtos;

namespace Mes.System.Security.Dtos;
public sealed partial class GetAllAspNetUsersQuery : IRequest<GetAllAspNetUsersQueryResult>
{
    public GetAllAspNetUsersQuery()
    {
    }
}