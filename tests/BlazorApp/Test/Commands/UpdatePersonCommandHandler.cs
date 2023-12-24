using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Commands;
public sealed partial class UpdatePersonCommandHandler
{
    public Task<UpdatePersonCommandResult> HandleAsync(UpdatePersonCommand command)
    {
        var firstName = command.Params.FirstName?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{command.Params.FirstName.ToString()}'";
        var lastName = $"N'{command.Params.LastName.ToString()}'";
        var dateOfBirth = $"N'{SqlTypeHelper.FormatDate(command.Params.DateOfBirth)}'";
        var height = command.Params.Height?.ToString() ?? "null";
        var dbCommand = $@"UPDATE [Person]   SET [FirstName] = {firstName}, [LastName] = {lastName}, [DateOfBirth] = {dateOfBirth}, [Height] = {height}   WHERE [ID] = {command.Params.Id}";
        var dbResult = this._sql.ExecuteScalarCommand(dbCommand);
        var result = new UpdatePersonCommandResult(new());
        return Task.FromResult(result);
    }
}