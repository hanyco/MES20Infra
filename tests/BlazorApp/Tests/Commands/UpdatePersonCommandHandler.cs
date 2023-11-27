using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Commands;
public sealed partial class UpdatePersonCommandHandler
{
    public Task<UpdatePersonCommandResult> HandleAsync(UpdatePersonCommand command)
    {
        var dbCommand = $@"Update [Person]   SET ([FirstName], [LastName], [DateOfBirth], [Height])   VALUES (N'{command.Params.FirstName}', N'{command.Params.LastName}', N'{command.Params.DateOfBirth}', {command.Params.Height})   WHERE [ID] = {command.Params.Id}";
        var dbResult = this._sql.ExecuteScalarCommand(dbCommand);
        int id = Convert.ToInt32(dbResult);
        var result = new UpdatePersonCommandResult(new() { Id = id });
        return Task.FromResult(result);
    }
}