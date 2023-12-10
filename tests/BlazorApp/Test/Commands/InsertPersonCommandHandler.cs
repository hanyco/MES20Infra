using System.Threading.Tasks;

using HumanResources.Dtos;
namespace HumanResources.Commands;
public sealed partial class InsertPersonCommandHandler
{
    public Task<InsertPersonCommandResult> HandleAsync(InsertPersonCommand command)
    {
        var a = command.Params.FirstName?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{command.Params.FirstName.ToString()}'";
        var b = $"N'{command.Params.LastName}'";
        var c = command.Params.DateOfBirth.ToString().IsNullOrEmpty() ? "null" : $"N'{SqlTypeHelper.FormatDate(command.Params.DateOfBirth)}'";
        var d = command.Params.Height?.ToString() ?? "null";
        var dbCommand = $@"INSERT INTO [Person]   ([FirstName], [LastName], [DateOfBirth], [Height])   VALUES ({a}, {b}, {c}, {d}); SELECT SCOPE_IDENTITY()";
        var dbResult = this._sql.ExecuteScalarCommand(dbCommand);
        int id = Convert.ToInt32(dbResult);
        var result = new InsertPersonCommandResult(new() { Id = id });
        return Task.FromResult(result);
    }
}
