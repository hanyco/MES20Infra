using Contracts.Services;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.Services.Imp;
using HanyCo.Infra.UI.ViewModels;

using Library.BusinessServices;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

[Service]
internal sealed class CqrsCommandService(
    InfraReadDbContext readDbContext,
    InfraWriteDbContext writeDbContext,
    IEntityViewModelConverter converter,
    ISecurityService securityService) : CqrsSegregationServiceBase,
    ICqrsCommandService,
    IAsyncValidator<CqrsCommandViewModel>,
    IResetChanges,
    IAsyncRead<CqrsCommandViewModel>
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly ISecurityService _securityService = securityService;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;

    protected override CqrsSegregateType SegregateType { get; } = CqrsSegregateType.Command;

    public Task<CqrsCommandViewModel> CreateAsync(CancellationToken token = default)
        => Task.FromResult(new CqrsCommandViewModel { HasPartialHandler = true, HasPartialOnInitialize = true });

    public async Task<Result> DeleteAsync(CqrsCommandViewModel model, bool persist = true, CancellationToken token = default)
    {
        Check.MustBeArgumentNotNull(model?.Id);
        var entry = this._writeDbContext.Attach(new CqrsSegregate { Id = model.Id.Value });
        _ = this._writeDbContext.Remove(entry.Entity);
        _ = await this._securityService.RemoveEntityClaimsAsync(model.Guid!.Value, persist, token);
        return await this.SubmitChangesAsync(persist: persist, token: token);
    }

    public Task<CqrsCommandViewModel> FillByDbEntity(
        CqrsCommandViewModel model,
        long id,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null, CancellationToken cancellationToken = default)
        => this.FillViewModelAsync(model, moduleName, paramDtoName, resultDtoName, cancellationToken);

    public async Task<CqrsCommandViewModel> FillViewModelAsync(
        CqrsCommandViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null,
        CancellationToken token = default)
    {
        var result = await ServiceHelper.GetByIdAsync(this, model.Id!.Value, this.GetAllQuery()
                       .Include(x => x.Module).Include(x => x.ParamDto)
                       .Include(x => x.ResultDto), x => this._converter.ToViewModel(x).Cast().As<CqrsCommandViewModel>(), this._readDbContext.AsyncLock);
        if (result?.Guid is { } guid)
        {
            result.SecurityClaims.AddRange( await this._securityService.GetEntityClaimsAsync(guid, token).GetValueAsync());
        }

        return result;
    }

    public Task<IReadOnlyList<CqrsCommandViewModel>> GetAllAsync(CancellationToken token = default)
        => ServiceHelper.GetAllAsync(
            this,
            this.GetAllQuery(),
            x => this._converter.ToViewModel(x).Cast<CqrsCommandViewModel>(),
            this._readDbContext.AsyncLock);

    public async Task<CqrsCommandViewModel?> GetByIdAsync(long id, CancellationToken token = default) =>
        await this.FillViewModelAsync(new() { Id = id }, token: token);

    public async Task<Result<CqrsCommandViewModel>> InsertAsync(CqrsCommandViewModel model, bool persist = true, CancellationToken token = default)
    {
        var man = ServiceHelper.InsertAsync(this,
            this._writeDbContext,
            model,
            this._converter.ToDbEntity,
            persist,
            onCommitted: (m, e) => m.Id = e.Id,
            cancellationToken: token);
        var result = await man.ModelResult();
        if (result.IsSucceed)
        {
            var buffer = result.Value;
            _ = await this._securityService.SetEntityClaimsAsync(buffer.Guid!.Value, buffer.SecurityClaims, persist, token);
        }

        return result;
    }

    public void ResetChanges() =>
        this._writeDbContext.ResetChanges();

    public async Task<Result<int>> SaveChangesAsync(CancellationToken token = default) =>
        await this._writeDbContext.SaveChangesResultAsync(cancellationToken: token);

    public async Task<Result<CqrsCommandViewModel>> UpdateAsync(long id, CqrsCommandViewModel model, bool persist = true, CancellationToken token = default)
    {
        _ = await this.ValidateAsync(model, token);
        Check.MustBeArgumentNotNull(model.Id);
        var segregate = this._converter.ToDbEntity(model)!;
        _ = this._writeDbContext.Attach(segregate)
            .SetModified(x => x.Comment)
            .SetModified(x => x.Description)
            .SetModified(x => x.FriendlyName)
            .SetModified(x => x.Name)
            .SetModified(x => x.ParamDtoId)
            .SetModified(x => x.ResultDtoId)
            .SetModified(x => x.ModuleId)
            .SetModified(x => x.Comment)
            .SetModified(x => x.SegregateType)
            .SetModified(x => x.CategoryId)
            .SetModified(x => x.CqrsNameSpace);
        _ = await this._securityService.SetEntityClaimsAsync(model.Guid!.Value, model.SecurityClaims, persist, token);
        _ = await this.SubmitChangesAsync(persist: persist, token: token);
        return Result<CqrsCommandViewModel>.CreateSuccess(model);
    }

    public Task<Result<CqrsCommandViewModel>> ValidateAsync(CqrsCommandViewModel? item, CancellationToken token = default)
        => item.ArgumentNotNull().Check()
            .NotNull(x => x.Name)
            .NotNull(x => x.ParamsDto)
            .NotNull(x => x.ResultDto)
            .Build().ToAsync();

    private IQueryable<CqrsSegregate> GetAllQuery()
    {
        var type = CqrsSegregateType.Command.Cast().ToInt();
        var query = from cmd in this._readDbContext.CqrsSegregates
                    where cmd.SegregateType == type
                    select cmd;
        return query;
    }
}