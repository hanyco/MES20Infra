using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;
using MediatR;

namespace HumanResources.Dtos;
public sealed partial class GetAllPeopleQuery : IRequest<GetAllPeopleQueryResult>
{
    public PersonDto PersonDto { get; set; }

    public GetAllPeopleQuery(PersonDto personDto)
    {
        this.PersonDto = personDto;
    }
}