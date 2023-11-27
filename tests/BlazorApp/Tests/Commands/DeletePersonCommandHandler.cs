using HumanResources.Dtos;

namespace HumanResources.Commands;

public sealed partial class DeletePersonCommandHandler
{
    public Task<DeletePersonCommandResult> HandleAsync(DeletePersonCommand command)
    {
        var dbCommand = $"DELETE FROM [dbo].[Person] WHERE [Id] = {command.Params.Id}";
        this._sql.ExecuteNonQuery(dbCommand);
        var result = new DeletePersonCommandResult(new());
        return Task.FromResult(result);
    }
}