using System.Threading.Tasks;
using HumanResource.Dtos;

namespace HumanResource.Commands;
public sealed partial class UpdatePersonCommandHandler
{
    public Task<UpdatePersonCommandResult> Handle(UpdatePersonCommand command)
    {
        var firstName = command.Params.FirstName?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{command.Params.FirstName.ToString()}'";
        var lastName = $"N'{command.Params.LastName.ToString()}'";
        var dateOfBirth = $"N'{SqlTypeHelper.FormatDate(command.Params.DateOfBirth)}'";
        var height = command.Params.Height?.ToString() ?? "null";
        var dbCommand = $@"Library.Data.SqlServer.SqlStatementBuilder+UpdateStatement";
        var dbResult = this._sql.ExecuteScalarCommand(dbCommand);
        var result = new UpdatePersonCommandResult(new());
        return Task.FromResult(result);
    }
}