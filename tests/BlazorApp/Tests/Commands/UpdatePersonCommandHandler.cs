using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Commands;
public sealed partial class UpdatePersonCommandHandler
{
    public Task<UpdatePersonCommandResult> HandleAsync(UpdatePersonCommand command)
    {
        var dbCommand = $"UPDATE Employees SET Name = @Name, Position = @Position WHERE ID = {command.Params.Id}";
        this._sql.ExecuteNonQuery(dbCommand);
        var result = new UpdatePersonCommandResult(new());
        return Task.FromResult(result);
    }
}