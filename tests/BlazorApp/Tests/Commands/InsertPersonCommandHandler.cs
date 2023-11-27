using System.Data;
using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Commands;
public sealed partial class InsertPersonCommandHandler
{
    public Task<InsertPersonCommandResult> HandleAsync(InsertPersonCommand command)
    {
        var dbCommand = $"INSERT INTO Employees (Name, Position) VALUES (@Name, @Position); SELECT SCOPE_IDENTITY();";
        var dbResult = this._sql.ExecuteScalarCommand(dbCommand);
        int id = Convert.ToInt32(dbResult);
        var result = new InsertPersonCommandResult(new(id));
        return Task.FromResult(result);
    }
}