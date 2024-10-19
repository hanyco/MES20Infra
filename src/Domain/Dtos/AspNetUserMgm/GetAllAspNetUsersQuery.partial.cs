using MediatR;
using Mes.HumanResources.Dtos;

namespace Mes.HumanResources.Dtos;
public sealed partial class GetAllAspNetUsersQuery : IRequest<GetAllAspNetUsersQueryResult>
{
    public GetAllAspNetUsersQuery()
    {
    }
}