
using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.EntityFrameworkCore;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

using Services.Helpers;

namespace Services;

internal sealed class BlazorPageService(
    InfraReadDbContext readDbContext,
    InfraWriteDbContext writeDbContext,
    IEntityViewModelConverter converter,
    ILogger logger) : IBlazorPageService
{
    private readonly IEntityViewModelConverter _converter = converter;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;

    public ILogger Logger { get; } = logger;

    public UiPageViewModel? CreateViewModel(
        DtoViewModel dto,
        string? name = null,
        ModuleViewModel? module = null,
        string? nameSpace = null,
        Guid? guid = null,
        string? route = null,
        DbObjectViewModel? propertyDbObject = null)
    {
        if (dto == null)
        {
            return null;
        }
        var pureName = CommonHelpers.Purify(name ?? dto.Name);
        var result = new UiPageViewModel
        {
            Name = pureName?.AddEnd("Page"),
            ClassName = pureName?.AddEnd("Page"),
            Guid = guid ?? Guid.NewGuid(),
            GenerateMainCode = true,
            GeneratePartialCode = true,
            GenerateUiCode = true,
            Module = module ?? dto.Module,
            NameSpace = nameSpace ?? $"{CommonHelpers.Purify(dto.NameSpace)}.Pages"
        };
        if (!route.IsNullOrEmpty())
        {
            result.Routes.Add(route);
        }

        if (dto.IsViewModel)
        {
            result.DataContext = dto;
        }
        else
        {
            var listProp = new PropertyViewModel
            {
                Dto = dto,
                Type = PropertyType.Dto,
                IsList = true,
                IsNullable = true,
                Name = $"{StringHelper.Pluralize(pureName)}ListDto",
                DbObject = propertyDbObject
            };
            var detailsProp = new PropertyViewModel
            {
                Dto = dto,
                Type = PropertyType.Dto,
                IsList = false,
                IsNullable = true,
                Name = $"{pureName}DetailsDto",
                DbObject = propertyDbObject
            };
            result.DataContext = dto.IsViewModel ? dto : new()
            {
                Name = $"{pureName}ViewModel",
                IsViewModel = true,
                DbObject = dto.DbObject,
                Guid = dto.Guid,
                IsList = dto.IsList,
                NameSpace = dto.NameSpace,
                Module = dto.Module
            };
            _ = result.DataContext.Properties.AddRange(new[] { listProp, detailsProp });
        }
        return result;
    }

    public Task<Result> DeleteAsync(UiPageViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        DataServiceHelper.DeleteAsync<UiPageViewModel, UiPage>(this, this._writeDbContext, model, persist, null, this.Logger);

    public Task<IReadOnlyList<UiPageViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => DataServiceHelper.GetAllAsync<UiPageViewModel, UiPage>(this, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<UiPageViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => DataServiceHelper.GetByIdAsync(this, id, this._readDbContext.UiPages.Include(x => x.Dto).Include(x => x.Module).Include(x => x.UiPageComponents).ThenInclude(x => x.UiComponent).Include(x => x.UiPageComponents).ThenInclude(x => x.UiComponent.PageDataContext).Include(x => x.UiPageComponents).ThenInclude(x => x.UiComponent.PageDataContextProperty).Include(x => x.UiPageComponents).ThenInclude(x => x.Position), this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<Result<UiPageViewModel>> InsertAsync(UiPageViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        DataServiceHelper.InsertAsync(this, this._writeDbContext, model, this._converter.ToDbEntity, x => this.Validate(x), persist, onCommitted: (m, e) => m.Id = e.Id, cancellationToken: cancellationToken).ModelResult();

    public void ResetChanges()
        => this._writeDbContext.ResetChanges();

    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
        => this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public async Task<Result<UiPageViewModel>> UpdateAsync(long id, UiPageViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        if (!this.Validate(model).TryParse(out var validation))
        {
            return validation;
        }
        model.Id = id;
        var entity = this._converter.ToDbEntity(model)!;
        try
        {
            var entry = this._writeDbContext.Attach(entity)
                .SetModified(x => x.ClassName)
                .SetModified(x => x.ModuleId)
                .SetModified(x => x.Name)
                .SetModified(x => x.NameSpace)
                .SetModified(x => x.Route);

            // delete components that are removed from entity
            var pageCompIds = entity.UiPageComponents.Select(y => y.UiComponentId);
            var query = from x in this._readDbContext.UiPageComponents
                        where !pageCompIds.Contains(x.UiComponentId) && x.PageId == id
                        select x.Id;
            var removedComponents = await query.ToListLockAsync(this._readDbContext.AsyncLock);
            _ = this._writeDbContext.RemoveById<UiPageComponent>(removedComponents);

            var save = await this.SubmitChangesAsync(persist, token: cancellationToken);
            var result = (await this.GetByIdAsync(entity.Id, cancellationToken: cancellationToken))!;
            return Result.From<UiPageViewModel>(save, result);
        }
        finally
        {
            this.ResetChanges();
        }
    }

    public Result<UiPageViewModel> Validate(in UiPageViewModel? model) =>
        model.ArgumentNotNull().Check()
             .NotNull(x => x.Name)
             .NotNull(x => x.NameSpace)
             .NotNull(x => x.ClassName)
             .NotNull(x => x.DataContext)
             .NotNull(x => x.Module)
             //.NotNull(x => x.Route)
             .Build();
}