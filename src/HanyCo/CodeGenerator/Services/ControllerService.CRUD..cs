using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.EntityFrameworkCore;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal partial class ControllerService
    : IAsyncSaveChanges, IResetChanges, IAsyncValidator<ControllerViewModel>
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly ISecurityService _securityService = securityService;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;
    public Task<Result<int>> DeleteAsync(ControllerViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        this.Delete<ControllerViewModel, Controller>(this._writeDbContext, model, persist);

    public async Task<Result> DeleteById(long controllerId, bool persist = true, CancellationToken token = default)
    {
        var entry = this._writeDbContext.ReAttach(new CqrsSegregate { Id = controllerId }).Entry;
        _ = this._writeDbContext.Remove(entry.Entity);
        return await this.SubmitChanges(persist: persist, token: token);
    }

    public Task<IReadOnlyList<ControllerViewModel>> GetAllAsync(CancellationToken cancellationToken = default) =>
        this.GetAll<ControllerViewModel, Controller>(this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public async Task<ControllerViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var query = from entity in this._readDbContext.Controllers
                        .Include(x => x.ControllerMethods)
                        .Include(x => x.Module)
                    where entity.Id == id
                    select entity;
        var dbResult = await query.FirstOrDefaultLockAsync(this._readDbContext.AsyncLock, cancellationToken: cancellationToken);
        var result = this._converter.ToViewModel(dbResult);
        return result;
    }

    public async Task<Result<ControllerViewModel>> InsertAsync(ControllerViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        var validationCheck = await this.ValidateAsync(model, cancellationToken);
        if (!validationCheck.IsSucceed)
        {
            return validationCheck!;
        }

        var controllerEntity = this._converter.ToDbEntity(model);
        await using var transaction = await this._writeDbContext.Database.BeginTransactionAsync(cancellationToken);
        _ = await this._writeDbContext.Controllers.AddAsync(controllerEntity, cancellationToken);

        var apis = new List<(ControllerMethodViewModel Model, ControllerMethod Entity)>();
        foreach (var api in model.Apis)
        {
            var apiEntity = this._converter.ToDbEntity(api).With(x => x.Controller = controllerEntity);
            _ = await this._writeDbContext.ControllerMethods.AddAsync(apiEntity, cancellationToken);
            apis.Add((api, apiEntity));
        }
        var result = await this.SubmitChanges(persist, transaction, token: cancellationToken).OnSucceed(r =>
        {
            model.Id = controllerEntity.Id;
            foreach (var (Model, Entity) in apis)
            {
                Model.Id = Entity.Id;
            }
        });

        return Result.From(result, model);
    }

    public void ResetChanges() =>
        this._writeDbContext.ResetChanges();

    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public async Task<Result<ControllerViewModel>> UpdateAsync(long id, ControllerViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        var vr = await this.ValidateAsync(model, cancellationToken);
        if (!vr.IsSucceed)
        {
            return vr;
        }

        var entity = this._converter.ToDbEntity(model);
        _ = this._writeDbContext.Attach(entity)
            .SetModified(x => x.AdditionalUsings)
            .SetModified(x => x.ControllerName)
            .SetModified(x => x.ControllerRoute)
            .SetModified(x => x.CtorParams)
            .SetModified(x => x.IsAnonymousAllow)
            .SetModified(x => x.ModuleId)
            .SetModified(x => x.NameSpace);
        _ = await this._securityService.SetEntityClaims(model.Guid!.Value, model.SecurityClaims, persist, cancellationToken);
        _ = await this.SubmitChanges(persist: persist, token: cancellationToken);
        return Result.Success(model);
    }

    public Task<Result<ControllerViewModel>> ValidateAsync(ControllerViewModel? item, CancellationToken token = default) => item.ArgumentNotNull()
            .Check()
            .NotNull(x => x.Name)
            .NotNull(x => x.NameSpace)
            .NotNull(x => x.Module)
            .Build().ToAsync();
}