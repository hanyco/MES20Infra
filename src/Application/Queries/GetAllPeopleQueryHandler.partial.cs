using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Queries;
internal sealed partial class GetAllPeopleQueryHandler : IRequestHandler<GetAllPeopleQuery, GetAllPeopleQueryResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public GetAllPeopleQueryHandler(IMediator mediator, Sql sql)
    {
        this._mediator = mediator;
        this._sql = sql;
    }

    public async Task<GetAllPeopleQueryResult> Handle(GetAllPeopleQuery request, CancellationToken cancellationToken)
    {
        var dbQuery = $@"SELECT [Id], [FirstName], [LastName], [DateOfBirth], [Height]   FROM [dbo].[Person]";
        var dbResult = await this._sql.SelectAsync<PersonDto>(dbQuery).ToListAsync(cancellationToken);
        var result = new GetAllPeopleQueryResult(dbResult);
        return result;
    }
}