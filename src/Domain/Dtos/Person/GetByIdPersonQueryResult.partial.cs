namespace Mes.HumanResourcesManagement.Dtos;
public sealed partial class GetByIdPersonQueryResult
{
    public PersonDto PersonDto { get; set; }

    public GetByIdPersonQueryResult(PersonDto personDto)
    {
        this.PersonDto = personDto;
    }
}