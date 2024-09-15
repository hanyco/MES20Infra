using MediatR;

namespace HumanResources.Dtos;
public sealed partial class GetAllPeople : IRequest<GetAllPeopleQueryResult>
{
}