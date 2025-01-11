using MediatR;
using Mes.HumanResourcesManagement.Dtos;

namespace Mes.HumanResourcesManagement.Dtos;
public sealed partial class GetAllPeopleQuery : IRequest<GetAllPeopleQueryResult>
{
    public GetAllPeopleQuery()
    {
    }
}