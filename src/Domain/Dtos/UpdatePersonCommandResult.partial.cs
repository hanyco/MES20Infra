using HumanResource.Dtos;

namespace HumanResource.Dtos;
public sealed class UpdatePersonCommandResult
{
    public UpdatePersonCommandResult(UpdatePersonResult result)
    {
        this.Result = result;
    }

    public UpdatePersonResult Result { get; set; }
}