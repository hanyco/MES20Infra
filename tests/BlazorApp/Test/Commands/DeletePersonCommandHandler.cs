namespace Test.HumanResources.Commands;

public sealed partial class DeletePersonCommandHandler
{
    public Task<DeletePersonCommandResult> HandleAsync(DeletePersonCommandParams command) => 
        Task.FromResult<DeletePersonCommandResult>(null!);
}