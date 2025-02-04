﻿using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Results;
using Library.Types;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.UI.Services.Imp;

internal sealed class SecurityDescriptorService : IBusinesService, ISecurityDescriptorService,
    IAsyncSaveService, IResetChanges
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

    public IAsyncEnumerable<SecurityDescriptorViewModel> AssignToEntityIdAsync(Guid entityId, IEnumerable<Id>? securityDescriptorIds, bool persist = true) =>
        throw new NotImplementedException();

    public async Task<Result> DeleteAsync(SecurityDescriptorViewModel model, bool persist = true)
    {
        Check.ArgumentNotNull(model);
        _ = this._writeDbContext.SecurityDescriptors.Remove(new() { Id = model.Id });
        return (Result)(persist && (await this._writeDbContext.SaveChangesAsync()) > 0);
    }

    public Task DeleteByEntityIdAsync(Guid entityId, bool persist = true) => Task.CompletedTask;

    public async Task<bool> ExistsById(Guid id)
    {
        var query = from x in this.SelectAll()
                    where x.Id == id
                    select x.Id;
        var dbResult = await query.AnyAsync();
        return dbResult;
    }

    public async Task<IReadOnlyList<SecurityDescriptorViewModel>> GetAllAsync()
    {
        var query = this.SelectAll();
        var dbResult = await query.ToListAsync();
        var result = this._converter.ToViewModel(dbResult).Compact();
        return result.ToReadOnlyList();
    }

    public async Task<IEnumerable<SecurityDescriptorViewModel>> GetByEntityIdAsync(Guid entityId)
    {
        var query = this.SelectByEntityId(entityId);
        var dbResult = await query.ToListAsync();
        var result = this._converter.ToViewModel(dbResult).Compact();
        return result;
    }

    public async Task<SecurityDescriptorViewModel?> GetByIdAsync(Id id)
    {
        var query = this.SelectFullAll().Where(x => x.Id == id.Value);
        var dbResult = await query.FirstOrDefaultAsync();
        var result = this._converter.ToViewModel(dbResult);
        return result;
    }

    public async Task<Result<SecurityDescriptorViewModel>> InsertAsync(SecurityDescriptorViewModel model, bool persist = true)
    {
        _ = await this.CheckValidatorAsync(model);
        var entity = await insertSecurityDescriptor(model, false);
        //await this.MaintainStrategy(model);
        _ = await this.SubmitChangesAsync(persist: persist);
        model.Id = entity.Id;

        return Result<SecurityDescriptorViewModel>.CreateSuccess(model);

        async Task<SecurityDescriptor> insertSecurityDescriptor(SecurityDescriptorViewModel model, bool persist)
        {
            var (_, (_, e)) = await this.InsertAsync<SecurityDescriptorViewModel, SecurityDescriptor, Guid>(
                this._writeDbContext,
                model,
                this._converter.ToDbEntity,
                this.ValidateAsync,
                persist,
                onCommitted: (m, e) => m.Id = e.Id);
            e.EntitySecurities.Clear();
            return e;
        }
    }

    public void ResetChanges() =>
            this._writeDbContext.ChangeTracker.Clear();

    public async Task<Result<int>> SaveChangesAsync()
        => await this._writeDbContext.SaveChangesResultAsync();

    //ToDo: Not done yet.
    public Task UnassignEntity(Guid entityId, IEnumerable<Id>? securityDescriptorIds = null, bool persist = true)
        => Task.CompletedTask;

    public async Task<Result<SecurityDescriptorViewModel>> UpdateAsync(Id id, SecurityDescriptorViewModel model, bool persist = true)
    {
        _ = await this.CheckValidatorAsync(model);
        //x await removeOldClaims(model.Id);
        var entity = await updateSecurityDescriptor(model);
        _ = await this.SubmitChangesAsync(persist: persist);
        model.Id = entity.Id;

        return Result<SecurityDescriptorViewModel>.CreateSuccess(model);

        async Task<SecurityDescriptor> updateSecurityDescriptor(SecurityDescriptorViewModel model)
        {
            var (_, (_, e)) = await this.UpdateAsync<SecurityDescriptorViewModel, SecurityDescriptor, Guid>(
                this._writeDbContext,
                model,
                this._converter.ToDbEntity,
                this.ValidateAsync,
                x =>
                {
                    _ = this._writeDbContext.Attach(x).SetModified(y => y.Name).SetModified(y => y.IsEnabled).SetModified(y => y.Strategy);
                    _ = x.SecurityClaims.ForEachEager(c => this._writeDbContext.Attach(c).SetModified(y => y.ClaimType).SetModified(y => y.ClaimValue));
                    return x;
                },
                false);
            return e!;
        }
    }

    public Task<Result<SecurityDescriptorViewModel>> ValidateAsync([NotNull] SecurityDescriptorViewModel item)
    {
        Check.ArgumentNotNull(item);

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
            result = result.Check(item.ClaimSet?.Any() is true, $"{nameof(item.ClaimSet)} has no items.");
        }
        return result.ToResult().ToAsync();
    }

    //private async Task MaintainStrategy(SecurityDescriptorViewModel model)
    //{
    //    var task = model switch
    //    {
    //        { IsNoSec: true } => Task.CompletedTask,
    //        { IsClaimBased: true } => insertClaims(model),
    //        _ => Task.CompletedTask
    //    };
    //    await task;

    //    async Task insertClaims(SecurityDescriptorViewModel model)
    //    {
    //        foreach (var claim in model.ClaimSet)
    //        {
    //            _ = await this.InsertAsync<ClaimViewModel, SecurityClaim, Guid>(
    //                    this._writeDbContext,
    //                    claim,
    //                    this._converter.ToDbEntity,
    //                    persist: false,
    //                    onCommitting: x => x.Fluent(x.SecurityDescriptorId = model.Id));
    //        }
    //    }
    //}

    private IQueryable<SecurityDescriptor> SelectAll() =>
            from x in this._readDbContext.SecurityDescriptors
            select x;

    private IQueryable<SecurityDescriptor> SelectByEntityId(Guid entityId)
        => from x in this.SelectFullAll()
           from y in x.EntitySecurities
           where y.EntityId == entityId
           select x;

    private IQueryable<SecurityDescriptor> SelectFullAll() =>
        from x in this.SelectAll().Include(x => x.SecurityClaims).Include(x => x.EntitySecurities)
        select x;
}