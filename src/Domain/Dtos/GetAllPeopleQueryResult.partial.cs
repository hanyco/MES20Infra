using HumanResource.Dtos;

namespace HumanResource.Dtos;
public sealed partial class GetAllPeopleQueryResult
{
    public GetAllPeopleQueryResult(GetAllPeopleQueryResult result)
    {
        this.Result = result;
    }

    public GetAllPeopleQueryResult Result { get; set; }
}