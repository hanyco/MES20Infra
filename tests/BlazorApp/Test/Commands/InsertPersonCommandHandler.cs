using System.Threading.Tasks;

using HumanResources.Dtos;
namespace HumanResources.Commands;
public sealed partial class InsertPersonCommandHandler
{
    public Task<InsertPersonCommandResult> HandleAsync(InsertPersonCommand command)
    {
        var firstName = command.Params.FirstName?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{command.Params.FirstName.ToString()}'";
        var lastName = $"N'{command.Params.LastName.ToString()}'";
        var dateOfBirth = $"N'{SqlTypeHelper.FormatDate(command.Params.DateOfBirth)}'";
        var height = command.Params.Height?.ToString() ?? "null";
        var dbCommand = $@"INSERT INTO [Person]   ([FirstName], [LastName], [DateOfBirth], [Height])   VALUES ({firstName}, {lastName}, {dateOfBirth}, {height}); SELECT SCOPE_IDENTITY();";
        var dbResult = this._sql.ExecuteScalarCommand(dbCommand);
        int id = Convert.ToInt32(dbResult);
        var result = new InsertPersonCommandResult(new() { Id = id });
        return Task.FromResult(result);
    }
}
