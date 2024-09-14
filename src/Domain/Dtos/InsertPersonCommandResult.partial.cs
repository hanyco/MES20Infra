using HumanResource.Dtos;

namespace HumanResource.Dtos;
public sealed class InsertPersonCommandResult
{
    public InsertPersonCommandResult(InsertPersonResult result)
    {
        this.Result = result;
    }

    public InsertPersonResult Result { get; set; }
}