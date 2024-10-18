using MediatR;
using Mes.Security.Dtos;

namespace Mes.Security.Dtos;
public sealed partial class GetAllAspNetUsersQuery : IRequest<GetAllAspNetUsersQueryResult>
{
    public GetAllAspNetUsersQuery()
    {
    }
}