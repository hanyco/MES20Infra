using MediatR;
using Mes.HumanResources.Dtos;

namespace Mes.HumanResources.Dtos;
public sealed partial class GetAllPeopleQuery : IRequest<GetAllPeopleQueryResult>
{
    public GetAllPeopleQuery()
    {
    }
}