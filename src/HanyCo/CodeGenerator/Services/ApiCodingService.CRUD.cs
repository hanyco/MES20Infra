using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.EntityFrameworkCore;
using Library.Data.Markers;
using Library.Interfaces;
using Library.Results;
using Library.Validations;
using Library.Wpf.Bases;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

namespace Services;

internal partial class ControllerService : IAsyncSaveChanges, IResetChanges, IAsyncValidator<ControllerViewModel>
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;

    public Task<Result<int>> DeleteAsync(ControllerViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        this.DeleteAsync<ControllerViewModel, Controller>(_writeDbContext, model, persist);

    public Task<IReadOnlyList<ControllerViewModel>> GetAllAsync(CancellationToken cancellationToken = default) =>
        this.GetAllAsync<ControllerViewModel, Controller>(_readDbContext, _converter.ToViewModel, _readDbContext.AsyncLock);

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
        await _writeDbContext.Controllers.AddAsync(controllerEntity, cancellationToken);
        
        var apis = new List<(ControllerMethodViewModel Model, ControllerMethod Entity)>();
        foreach (var api in model.Apis)
        {
            var apiEntity = _converter.ToDbEntity(api).With(x => x.Controller = controllerEntity);
            await _writeDbContext.ControllerMethods.AddAsync(apiEntity, cancellationToken);
            apis.Add((api, apiEntity));
        }
        var result = await this.SubmitChangesAsync(persist, transaction, token: cancellationToken).IfSucceed(r =>
        {
            model.Id = controllerEntity.Id;
            foreach (var api in apis)
            {
                api.Model.Id = api.Entity.Id;
            }
        });
        
        return Result.From(result, model);
    }
    public void ResetChanges() => 
        _writeDbContext.ResetChanges();

    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public Task<Result<ControllerViewModel>> UpdateAsync(long id, ControllerViewModel model, bool persist = true, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public Task<Result<ControllerViewModel?>> ValidateAsync(ControllerViewModel? item, CancellationToken token = default) => throw new NotImplementedException();
}