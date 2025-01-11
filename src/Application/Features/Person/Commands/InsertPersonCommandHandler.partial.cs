using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using Mes.HumanResourcesManagement.Dtos;

namespace Mes.HumanResourcesManagement.Commands;
internal sealed partial class InsertPersonCommandHandler : IRequestHandler<InsertPersonCommand, InsertPersonCommandResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public InsertPersonCommandHandler(IMediator mediator, Sql sql)
    {
        this._mediator = mediator;
        this._sql = sql;
    }

    public async Task<InsertPersonCommandResult> Handle(InsertPersonCommand request, CancellationToken cancellationToken)
    {
        var firstName = request.Person.FirstName?.ToString().IsNullOrEmpty() ?? true ? "null" : $"N'{request.Person.FirstName.ToString()}'";
        var lastName = $"N'{request.Person.LastName.ToString()}'";
        var dateOfBirth = $"N{SqlTypeHelper.FormatDate(request.Person.DateOfBirth)}";
        var height = request.Person.Height?.ToString() ?? "null";
        var dbCommand = $@"INSERT INTO [dbo].[Person]   ([FirstName], [LastName], [DateOfBirth], [Height])   VALUES ({firstName}, {lastName}, {dateOfBirth}, {height}); SELECT SCOPE_IDENTITY();";
        var dbResult = await this._sql.ExecuteScalarCommandAsync(dbCommand, cancellationToken);
        var returnValue = Convert.ToInt64(dbResult);
        var result = new InsertPersonCommandResult(returnValue);
        return result;
    }
}