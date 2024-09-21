using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using HumanResources.Dtos;
using Microsoft.EntityFrameworkCore;
using Library.Helpers;

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
        var dbResult = await _sql.SelectAsync<PersonDto>("SELECT * FROM [dbo].[Person]").ToListAsync(cancellationToken);
        var response = new GetAllPeopleQueryResult(dbResult);
        return response;
    }
}