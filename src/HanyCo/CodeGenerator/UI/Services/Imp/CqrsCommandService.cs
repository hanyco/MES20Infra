using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Mapping;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.UI.Services.Imp;

internal sealed class CqrsCommandService : CqrsSegregationServiceBase, IBusinesService,
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
        : base(converter)
    {
        this._mapper = mapper;
        this._readDbContext = readDbContext;
        this._writeDbContext = writeDbContext;
        this._converter = converter;
    }

    protected override CqrsSegregateType SegregateType { get; } = CqrsSegregateType.Command;

    public Task<CqrsCommandViewModel> CreateAsync() => throw new NotImplementedException();

    public async Task<Result> DeleteAsync(CqrsCommandViewModel model, bool persist = true)
    {
        Check.ArgumentNotNull(model?.Id);
        var entry = this._writeDbContext.Attach(new CqrsSegregate { Id = model.Id.Value });
        _ = this._writeDbContext.Remove(entry.Entity);
        return (Result)(await this.SubmitChangesAsync(persist: persist) > 0);
    }

    public CqrsCommandViewModel FillByDbEntity(CqrsCommandViewModel model,
        CqrsSegregate sergregate,
        Module module,
        Dto parameterDto,
        IEnumerable<Property> parameterDtoProperties,
        Dto resultDto,
        IEnumerable<Property> resultDtoProperties)
    {
        _ = this._mapper.Map(sergregate, model);
        model.Module = this._converter.ToViewModel(module);
        model.ParamDto = this._converter.FillByDbEntity(parameterDto, parameterDtoProperties);
        model.ResultDto = this._converter.FillByDbEntity(resultDto, resultDtoProperties);
        return model;
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

    public async Task<IReadOnlyList<CqrsCommandViewModel>> GetCommandsByDtoIdAsync(long dtoId)
    {
        var query = from cmd in this.GetAllQuery()
                    where cmd.ParamDtoId == dtoId || cmd.ResultDtoId == dtoId
                    select cmd;
        var dbResult = await query.ToListAsync();
        var result = this._converter.ToViewModel(dbResult).Cast<CqrsCommandViewModel>().ToList();
        return result;
    }

    public Task<Result<CqrsCommandViewModel>> InsertAsync(CqrsCommandViewModel model, bool persist = true)
        => this.InsertAsync(this._writeDbContext, model, this._converter.ToDbEntity, persist).ModelResult();

    public void ResetChanges()
        => this._writeDbContext.ChangeTracker.Clear();

    public async Task<Result<int>> SaveChangesAsync()
        => await this._writeDbContext.SaveChangesResultAsync();

    public async Task<Result<CqrsCommandViewModel>> UpdateAsync(long id, CqrsCommandViewModel model, bool persist = true)
    {
        _ = await this.ValidateAsync(model);
        Check.ArgumentNotNull(model.Id);
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
        => (Check.MustBeNotNull(item, () => "Please fill the form.")
          + Check.MustBeNotNull(item, item.Name)
          + Check.MustBeNotNull(item, item.ParamDto?.Id)
          + Check.MustBeNotNull(item, item.ResultDto?.Id)).ToAsync();

    private IQueryable<CqrsSegregate> GetAllQuery()
    {
        var type = CqrsSegregateType.Command.ToInt();
        var query = from cmd in this._readDbContext.CqrsSegregates
                    where cmd.SegregateType == type
                    select cmd;
        return query;
    }
}