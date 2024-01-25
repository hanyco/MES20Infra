
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Results;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal class SecurityService(InfraReadDbContext readDbContext, InfraWriteDbContext infraWriteDb, IEntityViewModelConverter converter) : ISecurityService
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly InfraWriteDbContext _infraWriteDb = infraWriteDb;
    private readonly InfraReadDbContext _readDbContext = readDbContext;

    public Task<Result> DeleteAsync(ClaimViewModel model, bool persist = true, CancellationToken token = default) =>
        throw new NotImplementedException();

    public async Task<IReadOnlyList<ClaimViewModel>> GetAllAsync(CancellationToken token = default)
    {
        var query = from x in this._readDbContext.SecurityClaims
                    select x;
        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var result = this._converter.ToViewModel(dbResult);
        return result.Compact().ToReadOnlyList();
    }

    public async Task<ClaimViewModel?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var query = from x in this._readDbContext.SecurityClaims
                    where x.Id == id
                    select x;
        var dbResult = await query.FirstOrDefaultAsync(token);
        var result = this._converter.ToViewModel(dbResult);
        return result;
    }

    public async Task<Result<IEnumerable<ClaimViewModel>>> GetEntityClaimsAsync(Guid entity, CancellationToken token = default)
    {
        var query = from x in this._readDbContext.EntityClaims.Include(x => x.Claim)
                    where x.EntityId == entity
                    select x.Claim;
        var dbResult = await query.ToListAsync(cancellationToken: token);
        var result = this._converter.ToViewModel(dbResult).Compact();
        return Result<IEnumerable<ClaimViewModel>>.CreateSuccess(result);
    }

    public Task<Result<ClaimViewModel>> InsertAsync(ClaimViewModel model, bool persist = true, CancellationToken token = default) =>
        throw new NotImplementedException();

    public Task<Result> RemoveEntityClaimsAsync(Guid value, bool persist, CancellationToken token)
    {
        var result = CatchResult(() => this._infraWriteDb.EntityClaims.Where(x => x.EntityId == value).ForEach(x => this._infraWriteDb.EntityClaims.Remove(x)));
        return Task.FromResult(result);
    }

    public void ResetChanges() =>
        this._infraWriteDb.ResetChanges();

    public Task<Result<int>> SaveChangesAsync(CancellationToken token = default) => throw new NotImplementedException();

    public Task<Result> SetEntityClaimsAsync(Guid entity, IEnumerable<ClaimViewModel> claims, bool persist, CancellationToken token = default) => throw new NotImplementedException();

    public Task<Result<ClaimViewModel>> UpdateAsync(Guid id, ClaimViewModel model, bool persist = true, CancellationToken token = default) =>
        throw new NotImplementedException();
}