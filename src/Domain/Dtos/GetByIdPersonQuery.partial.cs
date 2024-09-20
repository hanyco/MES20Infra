using Library.Cqrs.Models.Queries;
using HumanResources.Dtos;

namespace HumanResources.Dtos;
public sealed partial class GetByIdPersonQuery : IQuery<GetByIdPersonQueryResult>
{
    public PersonDto PersonDto { get; set; }

    public GetByIdPersonQuery(PersonDto personDto)
    {
        this.PersonDto = personDto;
    }
}