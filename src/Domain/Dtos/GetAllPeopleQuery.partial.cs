using MediatR;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class GetAllPeopleQuery : IRequest<GetAllPeopleQueryResult>
{
    public GetAllPeopleQuery()
    {
    }
}