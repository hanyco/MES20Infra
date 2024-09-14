using HumanResource.Dtos;

namespace HumanResource.Dtos;
public sealed class GetByIdPersonQueryResult
{
    public GetByIdPersonQueryResult(GetByIdPersonResult result)
    {
        this.Result = result;
    }

    public GetByIdPersonResult Result { get; set; }
}