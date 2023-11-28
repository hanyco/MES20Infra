using Contracts.Services;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.BusinessServices;
using Library.Interfaces;
using Library.Results;
using Library.Types;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal sealed class SecurityDescriptorService : ISecurityDescriptorService,
    IAsyncSaveChanges, IResetChanges, IAsyncWrite<SecurityDescriptorViewModel>
{
    private readonly IEntityViewModelConverter _converter;
    private readonly InfraReadDbContext _readDbContext;
    private readonly InfraWriteDbContext _writeDbContext;

    public SecurityDescriptorService(InfraReadDbContext readDbContext, InfraWriteDbContext writeDbContext, IEntityViewModelConverter converter)
    {
        this._readDbContext = readDbContext;
        this._writeDbContext = writeDbContext;
        this._converter = converter;
    }

    public IAsyncEnumerable<SecurityDescriptorViewModel> AssignToEntityIdAsync(Guid entityId, IEnumerable<Id>? securityDescriptorIds, bool persist = true, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public async Task<Result> DeleteAsync(SecurityDescriptorViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(model);
        _ = this._writeDbContext.SecurityDescriptors.Remove(new() { Id = model.Id });
        return (Result)(persist && await this._writeDbContext.SaveChangesAsync(cancellationToken) > 0);
    }

    Task<Result> IAsyncWrite<SecurityDescriptorViewModel, long>.DeleteAsync(SecurityDescriptorViewModel model, bool persist, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task DeleteByEntityIdAsync(Guid entityId, bool persist = true, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

    public async Task<bool> ExistsById(Guid id, CancellationToken cancellationToken = default)
    {
        var query = from x in this.SelectAll()
                    where x.Id == id
                    select x.Id;
        var dbResult = await query.AnyAsync(cancellationToken: cancellationToken);
        return dbResult;
    }

    public async Task<IReadOnlyList<SecurityDescriptorViewModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        var query = this.SelectAll();
        var dbResult = await query.ToListAsync(cancellationToken: cancellationToken);
        var result = this._converter.ToViewModel(dbResult).Compact();
        return result.ToReadOnlyList();
    }

    public async Task<IEnumerable<SecurityDescriptorViewModel>> GetByEntityIdAsync(Guid entityId, CancellationToken cancellationToken = default)
    {
        var query = this.SelectByEntityId(entityId);
        var dbResult = await query.ToListAsync(cancellationToken: cancellationToken);
        var result = this._converter.ToViewModel(dbResult).Compact();
        return result;
    }

    public async Task<SecurityDescriptorViewModel?> GetByIdAsync(Id id, CancellationToken cancellationToken = default)
    {
        var query = this.SelectFullAll().Where(x => x.Id == id.Value);
        var dbResult = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        var result = this._converter.ToViewModel(dbResult);
        return result;
    }

    public async Task<Result<SecurityDescriptorViewModel>> InsertAsync(SecurityDescriptorViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        _ = await this.CheckValidatorAsync(model);
        //var entity = await insertSecurityDescriptor(model, false);
        //await this.MaintainStrategy(model);
        _ = await this.SubmitChangesAsync(persist: persist);
        //model.Id = entity.Id;

        return Result<SecurityDescriptorViewModel>.CreateSuccess(model, cancellationToken);

        //async Task<SecurityDescriptor> insertSecurityDescriptor(SecurityDescriptorViewModel model, bool persist)
        //{
        //    var (_, (_, e)) = await this.InsertAsync<SecurityDescriptorViewModel, SecurityDescriptor, Guid>(
        //        this._writeDbContext,
        //        model,
        //        this._converter.ToDbEntity,
        //        this.ValidateAsync,
        //        persist,
        //        onCommitted: (m, e) => m.Id = e.Id);
        //    e.EntitySecurities.Clear();
        //    return e;
        //}
    }

    Task<Result<SecurityDescriptorViewModel>> IAsyncWrite<SecurityDescriptorViewModel, long>.InsertAsync(SecurityDescriptorViewModel model, bool persist, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public void ResetChanges() =>
                this._writeDbContext.ChangeTracker.Clear();

    public async Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken)
        => await this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public async Task SetSecurityDescriptorsAsync<TEntity>(TEntity entity, bool persist = true, CancellationToken cancellationToken = default) where TEntity : IHasSecurityDescriptor
    {
        if (entity?.Guid is { } guid && entity.SecurityDescriptors?.Any() is true)
        {
            _ = await this.AssignToEntityIdAsync(guid, entity.SecurityDescriptors.Select(x => x.Id), false, cancellationToken).ToEnumerableAsync();
        }
    }

    //Undone
    public Task UnassignEntity(Guid entityId, IEnumerable<Id>? securityDescriptorIds = null, bool persist = true, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public async Task<Result<SecurityDescriptorViewModel>> UpdateAsync(Id id, SecurityDescriptorViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        _ = await this.CheckValidatorAsync(model);
        //x await removeOldClaims(model.Id);
        var entity = await updateSecurityDescriptor(model, cancellationToken);
        _ = await this.SubmitChangesAsync(persist: persist);
        model.Id = entity.Id;

        return Result<SecurityDescriptorViewModel>.CreateSuccess(model, cancellationToken);

        async Task<SecurityDescriptor> updateSecurityDescriptor(SecurityDescriptorViewModel model, CancellationToken cancellationToken = default)
        {
            var (_, (_, e)) = await ServiceHelper.UpdateAsync<SecurityDescriptorViewModel, SecurityDescriptor, Guid>(this, this._writeDbContext, model, this._converter.ToDbEntity, this.ValidateAsync, x =>
                {
                    _ = this._writeDbContext.Attach(x).SetModified(y => y.Name).SetModified(y => y.IsEnabled).SetModified(y => y.Strategy);
                    _ = x.SecurityClaims.ForEach(c => this._writeDbContext.Attach(c).SetModified(y => y.ClaimType).SetModified(y => y.ClaimValue));
                    return x;
                }, false, cancellationToken: cancellationToken);
            return e!;
        }
    }

    Task<Result<SecurityDescriptorViewModel>> IAsyncWrite<SecurityDescriptorViewModel, long>.UpdateAsync(long id, SecurityDescriptorViewModel model, bool persist, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<SecurityDescriptorViewModel?>> ValidateAsync(SecurityDescriptorViewModel? item, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(item);

        var result = item.Check()
            .NotNullOrEmpty(x => x.Name, () => "Security Descriptor name")
            .RuleFor((item.IsNoSec, item.IsClaimBased) switch
            {
                (false, false) => (_ => false, () => "Please select on security type, at least."),
                (true, true) => (_ => false, () => "Please select on security type, at least."),
                _ => (_ => true, () => string.Empty)
            });

        if (item.IsClaimBased)
        {
            //result = result.Check(item.ClaimSet?.Any() is true, $"{nameof(item.ClaimSet)} has no items.");
        }
        return result.Build().ToAsync();
    }

    private IQueryable<SecurityDescriptor> SelectAll()
        => from x in this._readDbContext.SecurityDescriptors
           select x;

    private IQueryable<SecurityDescriptor> SelectByEntityId(Guid entityId)
        => from x in this.SelectFullAll()
           from y in x.EntitySecurities
           where y.EntityId == entityId
           select x;

    private IQueryable<SecurityDescriptor> SelectFullAll()
        => from x in this.SelectAll().Include(x => x.SecurityClaims).Include(x => x.EntitySecurities)
           select x;
}