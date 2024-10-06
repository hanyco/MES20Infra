using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.EntityFrameworkCore;
using Library.Mapping;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal sealed class CqrsQueryService(
    IMapper mapper,
    InfraReadDbContext readDbContext,
    InfraWriteDbContext writeDbContext,
    IEntityViewModelConverter converter) : CqrsSegregationServiceBase, ICqrsQueryService, IValidator<CqrsQueryViewModel>
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly IMapper _mapper = mapper;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;

    protected override CqrsSegregateType SegregateType { get; } = CqrsSegregateType.Query;

    public Task<bool> AnyByNameAsync(string name)
    {
        var query = from entity in this.GetAllQuery()
                    where entity.Name == name
                    select entity.Id;
        return query.AnyAsync();
    }

    public Task<CqrsQueryViewModel> CreateAsync(CancellationToken token = default) =>
        Task.FromResult(new CqrsQueryViewModel { Category = CqrsSegregateCategory.Read, HasPartialHandler = true, HasPartialOnInitialize = true });

    public Task<Result<int>> DeleteAsync(CqrsQueryViewModel model, bool persist = true, CancellationToken token = default) =>
        DataServiceHelper.DeleteAsync<CqrsQueryViewModel, CqrsSegregate>(this, this._writeDbContext, model, persist, persist);

    public async Task<Result> DeleteByIdAsync(long id, bool persist = true, CancellationToken token = default)
    {
        _ = this._writeDbContext.RemoveById<CqrsSegregate>(id);
        return persist
            ? await this._writeDbContext.SaveChangesResultAsync(token)
            : Result.Success(-1);
    }

    public CqrsQueryViewModel FillByDbEntity(
        CqrsQueryViewModel @this,
        CqrsSegregate dbQuery,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null) =>
        this._mapper.Map(dbQuery, @this)
            .ForMember(x => x.Module = new(dbQuery.ModuleId, moduleName ?? dbQuery.Module.Name))
            .ForMember(x => x.ParamsDto = new(dbQuery.ParamDtoId, paramDtoName ?? dbQuery.ParamDto.Name) { NameSpace = dbQuery.ParamDto.NameSpace ?? string.Empty, IsList = dbQuery.ParamDto.IsList ?? false })
            .ForMember(x => x.ResultDto = new(dbQuery.ResultDtoId, resultDtoName ?? dbQuery.ResultDto.Name) { NameSpace = dbQuery.ResultDto.NameSpace ?? string.Empty, IsList = dbQuery.ResultDto.IsList ?? false })
            .ForMember(x => x.Category = CqrsSegregateCategory.Read);

    public CqrsQueryViewModel FillByDbEntity(
        CqrsQueryViewModel @this,
        CqrsSegregate segregate,
        HanyCo.Infra.Internals.Data.DataSources.Module infraModule,
        Dto parameterDto,
        IEnumerable<Property> parameterDtoProperties,
        Dto resultDto,
        IEnumerable<Property> resultDtoProperties)
    {
        _ = this._mapper.Map(segregate, @this);
        @this.Module = this._converter.ToViewModel(infraModule);
        @this.ParamsDto = this._converter.FillByDbEntity(parameterDto, parameterDtoProperties);
        @this.ResultDto = this._converter.FillByDbEntity(resultDto, resultDtoProperties);
        return @this;
    }

    public async Task<CqrsQueryViewModel> FillByDbEntity(
        CqrsQueryViewModel @this,
        long dbQueryId,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null, CancellationToken token = default)
    {
        var segrQuery = from cq in this._readDbContext.CqrsSegregates
                                                      .Include(x => x.Module)
                                                      .Include(x => x.ParamDto)
                                                      .Include(x => x.ResultDto)
                        where cq.Id == dbQueryId
                        select cq;
        var dbResult = await segrQuery.FirstOrDefaultAsync(cancellationToken: token);
        return this.FillByDbEntity(@this, dbResult.NotNull(() => "Query not found."), moduleName, paramDtoName, resultDtoName);
    }

    public async Task<CqrsQueryViewModel> FillViewModelAsync(CqrsQueryViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null, CancellationToken token = default)
    {
        var query = await getCQuery();

        var paramProps = await getProps(query.ParamsDto?.Id);
        var resultProps = await getProps(query.ResultDto?.Id);

        _ = query.ParamsDto?.Properties.AddRange(paramProps);
        _ = query.ResultDto?.Properties.AddRange(resultProps);

        return query;

        async Task<CqrsQueryViewModel> getCQuery()
        {
            var queriesQuery = from c in this.GetAllQuery().Include(x => x.Module).Include(x => x.ParamDto).Include(x => x.ResultDto)
                               where c.Id == model.Id
                               select c;
            var dbQuery = await queriesQuery.FirstOrDefaultAsync(cancellationToken: token);
            var cqrsQueryViewModel = this._converter.ToViewModel(dbQuery);
            return cqrsQueryViewModel.Cast().As<CqrsQueryViewModel>()!;
        }

        async Task<IEnumerable<PropertyViewModel>> getProps(long? paramDtoId)
        {
            if (query?.ParamsDto?.Id is null)
            {
                return Enumerable.Empty<PropertyViewModel>();
            }
            var paramPropsQuery = from p in this._readDbContext.Properties
                                  where p.ParentEntityId == paramDtoId
                                  select p;
            var dbParamProps = await paramPropsQuery.ToListAsync(cancellationToken: token);
            var paramProps = this._converter.ToViewModel(dbParamProps);
            return paramProps!;
        }
    }

    public async Task<IReadOnlyList<CqrsQueryViewModel>> GetAllAsync(CancellationToken token = default)
    {
        var query = this.GetAllQuery();
        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var result = this._converter.ToViewModel(dbResult).Cast<CqrsQueryViewModel>().ToList();
        return result;
    }

    public async Task<CqrsQueryViewModel?> GetByIdAsync(long id, CancellationToken token = default)
    {
        var query = from qry in this.GetAllQuery()
                    where qry.Id == id
                    select qry;
        var dbResult = await query.FirstOrDefaultAsync(cancellationToken: token);
        return this._converter.ToViewModel(dbResult).Cast().As<CqrsQueryViewModel>();
    }

    public async Task<IReadOnlyList<CqrsQueryViewModel>> GetQueriesByDtoIdAsync(long dtoId, CancellationToken token = default)
    {
        var query = from qry in this.GetAllQuery()
                    where qry.ParamDtoId == dtoId || qry.ResultDtoId == dtoId
                    select qry;
        var dbResult = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var result = this._converter.ToViewModel(dbResult).Cast<CqrsQueryViewModel>().ToList();
        return result;
    }

    public async Task<Result<CqrsQueryViewModel>> InsertAsync(CqrsQueryViewModel model, bool persist = true, CancellationToken token = default)
    {
        CqrsSegregate? segregate = null;
        try
        {
            if (!this.Validate(model).TryParse(out var vc))
            {
                return vc;
            }

            segregate = this._converter.ToDbEntity(model)!;

            _ = this._writeDbContext.Add(segregate);
            if (persist)
            {
                _ = await this._writeDbContext.SaveChangesAsync(cancellationToken: token);
            }

            model.Id = segregate.Id;
            return Result.Success<CqrsQueryViewModel>(model);
        }
        finally
        {
            if (segregate != null)
            {
                _ = this._writeDbContext.Detach(segregate);
            }
        }
    }

    public void ResetChanges() =>
        this._writeDbContext.ResetChanges();

    public async Task<Result<CqrsQueryViewModel>> UpdateAsync(long id, CqrsQueryViewModel model, bool persist = true, CancellationToken token = default)
    {
        CqrsSegregate? segregate = null;
        try
        {
            _ = this.Validate(model).ThrowOnFail();

            segregate = this._converter.ToDbEntity(model)!;

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
                    .SetModified(x => x.CqrsNameSpace);
            if (persist)
            {
                _ = await this._writeDbContext.SaveChangesAsync(cancellationToken: token);
            }

            model.Id = segregate.Id;
            return Result.Success<CqrsQueryViewModel>(model);
        }
        finally
        {
            _ = this._writeDbContext.Detach(segregate!);
        }
    }

    public Result<CqrsQueryViewModel> Validate(in CqrsQueryViewModel model)
        => model.Check()
                .NotNull()
                .NotNull(x => x.Name)
                .NotNull(x => x.ParamsDto)
                //.NotNull(x => x.ParamsDto.Id)
                .NotNull(x => x.ResultDto)
                //.NotNull(x => x.ResultDto.Id)
                .Build()!;

    private IQueryable<CqrsSegregate> GetAllQuery()
    {
        var type = CqrsSegregateType.Query.Cast().ToInt();
        var query = from qry in this._readDbContext.CqrsSegregates
                    where qry.SegregateType == type
                    select qry;
        return query;
    }
}