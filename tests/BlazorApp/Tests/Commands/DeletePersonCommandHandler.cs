using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Commands;
public sealed partial class DeletePersonCommandHandler
{
    public Task<DeletePersonCommandResult> HandleAsync(DeletePersonCommand command)
    {
        var dbCommand = $@"DELETE FROM [Person]   WHERE [ID] = {command.Params.Id}";
        this._sql.ExecuteNonQuery(dbCommand);
        var result = new DeletePersonCommandResult(new());
        return Task.FromResult(result);
    }
}