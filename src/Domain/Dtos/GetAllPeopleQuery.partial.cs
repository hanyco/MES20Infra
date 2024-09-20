using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class GetAllPeopleQuery : IQuery<GetAllPeopleQueryResult>
{
}