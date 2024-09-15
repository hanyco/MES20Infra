using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Commands;
public sealed partial class DeletePersonCommandHandler
{
    public Task<DeletePersonCommandResult> Handle(DeletePersonCommand command)
    {
        var dbCommand = $@"Library.Data.SqlServer.SqlStatementBuilder+DeleteStatement";
        this._sql.ExecuteNonQuery(dbCommand);
        var result = new DeletePersonCommandResult(new());
        return Task.FromResult(result);
    }
}