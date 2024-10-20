using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using Mes.Infra.Security.Dtos;

namespace Mes.Infra.Security.Queries;
internal sealed partial class GetByIdAspNetUserQueryHandler : IRequestHandler<GetByIdAspNetUserQuery, GetByIdAspNetUserQueryResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public GetByIdAspNetUserQueryHandler(IMediator mediator, Sql sql)
    {
        this._mediator = mediator;
        this._sql = sql;
    }

    public async Task<GetByIdAspNetUserQueryResult> Handle(GetByIdAspNetUserQuery request, CancellationToken cancellationToken)
    {
        var dbQuery = $@"SELECT TOP (1) [Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [DisplayName]   FROM [Identity].[AspNetUsers]   WHERE [Id] = {request.Id}";
        var dbResult = await this._sql.FirstOrDefaultAsync<AspNetUserDto>(dbQuery);
        var result = new GetByIdAspNetUserQueryResult(dbResult);
        return result;
    }
}