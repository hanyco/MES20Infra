using MediatR;
using Library.Data.SqlServer;
using System.Threading.Tasks;
using Mes.HumanResources.Dtos;

namespace Mes.HumanResources.Queries;
internal sealed partial class GetAllAspNetUsersQueryHandler : IRequestHandler<GetAllAspNetUsersQuery, GetAllAspNetUsersQueryResult>
{
    private readonly IMediator _mediator;
    private readonly Sql _sql;
    public GetAllAspNetUsersQueryHandler(IMediator mediator, Sql sql)
    {
        this._mediator = mediator;
        this._sql = sql;
    }

    public async Task<GetAllAspNetUsersQueryResult> Handle(GetAllAspNetUsersQuery request, CancellationToken cancellationToken)
    {
        var dbQuery = $@"SELECT [Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]   FROM [dbo].[AspNetUsers]";
        var dbResult = await this._sql.SelectAsync<AspNetUserDto>(dbQuery).ToListAsync(cancellationToken);
        var result = new GetAllAspNetUsersQueryResult(dbResult);
        return result;
    }
}