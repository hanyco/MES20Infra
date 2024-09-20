using MediatR;
using Library.Data.SqlServer;
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
}