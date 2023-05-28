using Contracts.Services;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.Services.Imp;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Mapping;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

[Service]
internal sealed class CqrsCommandService : CqrsSegregationServiceBase,
    ICqrsCommandService,
    IAsyncValidator<CqrsCommandViewModel>,
    IResetChanges,
    IAsyncRead<CqrsCommandViewModel>
{
    private readonly IEntityViewModelConverter _converter;
    private readonly IMapper _mapper;
    private readonly InfraReadDbContext _readDbContext;
    private readonly InfraWriteDbContext _writeDbContext;

    public CqrsCommandService(
        IMapper mapper,
        InfraReadDbContext readDbContext,
        InfraWriteDbContext writeDbContext,
        IEntityViewModelConverter converter)
    {
        this._mapper = mapper;
        this._readDbContext = readDbContext;
        this._writeDbContext = writeDbContext;
        this._converter = converter;
    }

    protected override CqrsSegregateType SegregateType { get; } = CqrsSegregateType.Command;

    public Task<CqrsCommandViewModel> CreateAsync(CancellationToken token = default)
        => Task.FromResult(new CqrsCommandViewModel { HasPartialHandller = true, HasPartialOnInitialize = true });

    public async Task<Result> DeleteAsync(CqrsCommandViewModel model, bool persist = true, CancellationToken token = default)
    {
        Check.IfArgumentNotNull(model?.Id);
        var entry = this._writeDbContext.Attach(new CqrsSegregate { Id = model.Id.Value });
        _ = this._writeDbContext.Remove(entry.Entity);
        return (Result)(await this.SubmitChangesAsync(persist: persist) > 0);
    }

    public Task<CqrsCommandViewModel> FillByDbEntity(
        CqrsCommandViewModel model,
        long id,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null, CancellationToken cancellationToken = default)
        => this.FillViewModelAsync(model, moduleName, paramDtoName, resultDtoName, cancellationToken);

    public Task<CqrsCommandViewModel> FillViewModelAsync(
        CqrsCommandViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null,
        CancellationToken token = default)
        => ServiceHelper.GetByIdAsync(this, model.Id!.Value, this.GetAllQuery()
               .Include(x => x.Module).Include(x => x.ParamDto)
               .Include(x => x.ResultDto), x => this._converter.ToViewModel(x).Cast().As<CqrsCommandViewModel>(), this._readDbContext.AsyncLock)!;

    public Task<IReadOnlyList<CqrsCommandViewModel>> GetAllAsync(CancellationToken token = default)
        => ServiceHelper.GetAllAsync(this, this.GetAllQuery(), x => this._converter.ToViewModel(x).Cast<CqrsCommandViewModel>(), this._readDbContext.AsyncLock);

    public Task<CqrsCommandViewModel?> GetByIdAsync(long id, CancellationToken token = default)
        => ServiceHelper.GetByIdAsync(this, id, this.GetAllQuery(), x => this._converter.ToViewModel(x).Cast().As<CqrsCommandViewModel>(), this._readDbContext.AsyncLock);

    public Task<Result<CqrsCommandViewModel>> InsertAsync(CqrsCommandViewModel model, bool persist = true, CancellationToken token = default)
        => ServiceHelper.InsertAsync(this, this._writeDbContext, model, this._converter.ToDbEntity, persist).ModelResult();

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public async Task<Result<int>> SaveChangesAsync(CancellationToken token = default)
        => await this._writeDbContext.SaveChangesResultAsync();

    public async Task<Result<CqrsCommandViewModel>> UpdateAsync(long id, CqrsCommandViewModel model, bool persist = true, CancellationToken token = default)
    {
        _ = await this.ValidateAsync(model, token);
        Check.IfArgumentNotNull(model.Id);
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
        _ = await this.SubmitChangesAsync(persist: persist);
        return Result<CqrsCommandViewModel>.CreateSuccess(model);
    }

    public Task<Result<CqrsCommandViewModel?>> ValidateAsync(CqrsCommandViewModel? item, CancellationToken token = default)
        => item.ArgumentNotNull().Check()
               .NotNull(x => x.Name)
               .NotNull(x => x.ParamDto)
               .NotNull(x => x.ParamDto.Id)
               .NotNull(x => x.ResultDto)
               .NotNull(x => x.ResultDto.Id)
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