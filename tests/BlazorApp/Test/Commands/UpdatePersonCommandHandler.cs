using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Commands;
public sealed partial class UpdatePersonCommandHandler
{
    public Task<UpdatePersonCommandResult> HandleAsync(UpdatePersonCommand command)
    {
        var dbCommand = $@"UPDATE [Person]   SET [FirstName] = N'{command.Params.FirstName}', [LastName] = N'{command.Params.LastName}', [DateOfBirth] = N'{SqlTypeHelper.FormatDate(command.Params.DateOfBirth)}', [Height] = {command.Params.Height}   WHERE [ID] = {command.Params.Id}";
        var dbResult = this._sql.ExecuteScalarCommand(dbCommand);
        var result = new UpdatePersonCommandResult(new());
        return Task.FromResult(result);
    }
}