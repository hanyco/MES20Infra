using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using HumanResources.Dtos;

namespace HumanResources.Queries;
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
        throw new NotImplementedException();
    }
}