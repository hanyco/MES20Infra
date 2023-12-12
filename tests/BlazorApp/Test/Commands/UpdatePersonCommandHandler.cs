using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Commands;
public sealed partial class UpdatePersonCommandHandler
{
    public Task<UpdatePersonCommandResult> HandleAsync(UpdatePersonCommand command)
    {
        var dbCommand = $@"";
        var dbResult = this._sql.ExecuteScalarCommand(dbCommand);
        var result = new UpdatePersonCommandResult(new());
        return Task.FromResult(result);
    }
}