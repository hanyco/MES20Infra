
using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;
using Library.Results;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal class SecurityService(
    InfraReadDbContext readDbContext,
    InfraWriteDbContext infraWriteDb,
    IEntityViewModelConverter converter) 
    : ISecurityService
    , IAsyncSaveChanges
    , IResetChanges
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly InfraWriteDbContext _infraWriteDb = infraWriteDb;
    private readonly InfraReadDbContext _readDbContext = readDbContext;

    public Task<Result<int>> DeleteAsync(ClaimViewModel model, bool persist = true, CancellationToken token = default) =>
        throw new NotImplementedException();

    public async Task<IReadOnlyList<ClaimViewModel>> GetAllAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ClaimViewModel?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        //var query = from x in this._readDbContext.SecurityClaims
        //            where x.Id == id
        //            select x;
        //var dbResult = await query.FirstOrDefaultAsync(token);
        //var result = this._converter.ToViewModel(dbResult);
        //return result;
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<ClaimViewModel>>> GetEntityClaims(Guid entity, CancellationToken token = default)
    {
        //var query = from x in this._readDbContext.EntityClaims.Include(x => x.Claim)
        //            where x.EntityId == entity
        //            select x.Claim;
        //var dbResult = await query.ToListAsync(cancellationToken: token);
        //var result = this._converter.ToViewModel(dbResult).Compact();
        //return Result.Success<IEnumerable<ClaimViewModel>>(result);
        throw new NotImplementedException();
    }

    public Task<Result<ClaimViewModel>> InsertAsync(ClaimViewModel model, bool persist = true, CancellationToken token = default) =>
        throw new NotImplementedException();

    public Task<Result> RemoveEntityClaims(Guid value, bool persist, CancellationToken token)
    {
        //var result = CatchResult(() => this._infraWriteDb.EntityClaims.Where(x => x.EntityId == value).ForEach(x => this._infraWriteDb.EntityClaims.Remove(x)));
        //return Task.FromResult(result);
        throw new NotImplementedException();
    }

    public void ResetChanges() =>
        this._infraWriteDb.ResetChanges();

    public Task<Result<int>> SaveChangesAsync(CancellationToken token = default) => throw new NotImplementedException();

    public Task<Result> SetEntityClaims(Guid entity, IEnumerable<ClaimViewModel> claims, bool persist, CancellationToken token = default)
    {
        //this._userManager.AddClaimAsync()
        return Task.FromResult(Result.Succeed);
    }

    public Task<Result<ClaimViewModel>> UpdateAsync(Guid id, ClaimViewModel model, bool persist = true, CancellationToken token = default) =>
        throw new NotImplementedException();
}