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
    IResetChanges
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

    public Task<CqrsCommandViewModel> CreateAsync()
        => Task.FromResult(new CqrsCommandViewModel { HasPartialHandller = true, HasPartialOnInitialize = true });

    public async Task<Result> DeleteAsync(CqrsCommandViewModel model, bool persist = true)
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
        string? resultDtoName = null)
        => this.FillViewModelAsync(model, moduleName, paramDtoName, resultDtoName);

    public Task<CqrsCommandViewModel> FillViewModelAsync(
        CqrsCommandViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null)
        => this.GetByIdAsync(model.Id!.Value, this.GetAllQuery()
               .Include(x => x.Module).Include(x => x.ParamDto)
               .Include(x => x.ResultDto), x => this._converter.ToViewModel(x).As<CqrsCommandViewModel>(), this._readDbContext.AsyncLock)!;

    public Task<IReadOnlyList<CqrsCommandViewModel>> GetAllAsync()
        => this.GetAllAsync(this.GetAllQuery(), x => this._converter.ToViewModel(x).Cast<CqrsCommandViewModel>(), this._readDbContext.AsyncLock);

    public Task<CqrsCommandViewModel?> GetByIdAsync(long id)
        => this.GetByIdAsync(id, this.GetAllQuery(), x => this._converter.ToViewModel(x).As<CqrsCommandViewModel>(), this._readDbContext.AsyncLock);

    public Task<Result<CqrsCommandViewModel>> InsertAsync(CqrsCommandViewModel model, bool persist = true)
        => this.InsertAsync(this._writeDbContext, model, this._converter.ToDbEntity, persist).ModelResult();

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public async Task<Result<int>> SaveChangesAsync()
        => await this._writeDbContext.SaveChangesResultAsync();

    public async Task<Result<CqrsCommandViewModel>> UpdateAsync(long id, CqrsCommandViewModel model, bool persist = true)
    {
        _ = await this.ValidateAsync(model);
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

    public Task<Result<CqrsCommandViewModel>> ValidateAsync(CqrsCommandViewModel item)
        => item.Check()
               .ArgumentNotNull()
               .NotNull(x=>x.Name)
               .NotNull(x => x.ParamDto)
               .NotNull(x => x.ParamDto.Id)
               .NotNull(x => x.ResultDto)
               .NotNull(x => x.ResultDto.Id)
               .Build().ToAsync();

    private IQueryable<CqrsSegregate> GetAllQuery()
    {
        var type = CqrsSegregateType.Command.ToInt();
        var query = from cmd in this._readDbContext.CqrsSegregates
                    where cmd.SegregateType == type
                    select cmd;
        return query;
    }
}