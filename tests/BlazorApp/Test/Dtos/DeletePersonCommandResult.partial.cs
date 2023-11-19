namespace Test.HumanResources.Dtos;
public sealed class DeletePersonCommandResult
{
    public DeletePersonCommandResult(DeletePersonResult result)
    {
        this.Result = result;
    }

    public DeletePersonResult Result { get; set; }
}