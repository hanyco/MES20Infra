using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using Mes.HumanResourcesManagement.Dtos;

namespace Mes.HumanResourcesManagement.Commands;
internal sealed partial class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, UpdatePersonCommandResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public UpdatePersonCommandHandler(IMediator mediator, Sql sql)
    {
        this._mediator = mediator;
        this._sql = sql;
    }

    public async Task<UpdatePersonCommandResult> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var firstName = request.Person.FirstName?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.Person.FirstName.ToString()}'";
        var lastName = $"N'{request.Person.LastName.ToString()}'";
        var dateOfBirth = $"N{SqlTypeHelper.FormatDate(request.Person.DateOfBirth)}";
        var height = request.Person.Height?.ToString() ?? "null";
        var dbCommand = $@"UPDATE [dbo].[Person]   SET [FirstName] = {firstName}, [LastName] = {lastName}, [DateOfBirth] = {dateOfBirth}, [Height] = {height}   WHERE [Id] = {request.Id}";
        var dbResult = await this._sql.ExecuteScalarCommandAsync(dbCommand, cancellationToken);
        var result = new UpdatePersonCommandResult();
        return result;
    }
}