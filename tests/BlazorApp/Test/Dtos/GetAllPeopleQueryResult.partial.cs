namespace Test.HumanResources.Dtos;
public sealed class GetAllPeopleQueryResult
{
    public GetAllPeopleQueryResult(List<GetAllPeopleResult> result)
    {
        this.Result = result;
    }

    public List<GetAllPeopleResult> Result { get; set; }
}