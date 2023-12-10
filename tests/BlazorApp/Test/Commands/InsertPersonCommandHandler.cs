using System.Threading.Tasks;

using HumanResources.Dtos;
namespace HumanResources.Commands;
public sealed partial class InsertPersonCommandHandler
{
    public Task<InsertPersonCommandResult> HandleAsync(InsertPersonCommand command)
    {
        var dbCommand = $@"INSERT INTO [Person]   ([FirstName], [LastName], [DateOfBirth], [Height])   VALUES (N'{command.Params.FirstName?.ToString() ?? DBNull.Value.ToString()}', N'{command.Params.LastName}', N'{SqlTypeHelper.FormatDate(command.Params.DateOfBirth)}', {command.Params.Height?.ToString() ?? DBNull.Value.ToString()}); SELECT SCOPE_IDENTITY()";
        var dbResult = this._sql.ExecuteScalarCommand(dbCommand);
        int id = Convert.ToInt32(dbResult);
        var result = new InsertPersonCommandResult(new() { Id = id });
        return Task.FromResult(result);
    }
}
