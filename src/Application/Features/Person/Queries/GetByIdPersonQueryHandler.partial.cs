using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using Mes.HumanResourcesManagement.Dtos;

namespace Mes.HumanResourcesManagement.Queries;
internal sealed partial class GetByIdPersonQueryHandler : IRequestHandler<GetByIdPersonQuery, GetByIdPersonQueryResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public GetByIdPersonQueryHandler(IMediator mediator, Sql sql)
    {
        this._mediator = mediator;
        this._sql = sql;
    }

    public async Task<GetByIdPersonQueryResult> Handle(GetByIdPersonQuery request, CancellationToken cancellationToken)
    {
        var dbQuery = $@"SELECT TOP (1) [Id], [FirstName], [LastName], [DateOfBirth], [Height]   FROM [dbo].[Person]   WHERE [Id] = {request.Id}";
        var dbResult = await this._sql.FirstOrDefaultAsync<PersonDto>(dbQuery);
        var result = new GetByIdPersonQueryResult(dbResult);
        return result;
    }
}