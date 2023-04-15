using Contracts.Services;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.CodeGeneration.FormGenerator.Blazor.Actors;
using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Validations;
using Library.Wpf.Bases;

using Microsoft.EntityFrameworkCore;

namespace HanyCo.Infra.UI.Services.Imp;

internal sealed class BlazorPageService : IBusinesService, IBlazorPageService
    //x, IAsyncViewModelFiller<UiPageViewModel>
    , IAsyncSaveService
    , IResetChanges
    , IAsyncValidator<UiPageViewModel>
    , ILoggerContainer
{
    private readonly IEntityViewModelConverter _converter;
    private readonly InfraReadDbContext _readDbContext;
    private readonly InfraWriteDbContext _writeDbContext;

    public BlazorPageService(InfraReadDbContext readDbContext,
                             InfraWriteDbContext writeDbContext,
                             IEntityViewModelConverter converter,
                             ILogger logger)
    {
        this._readDbContext = readDbContext;
        this._writeDbContext = writeDbContext;
        this._converter = converter;
        this.Logger = logger;
    }

    public ILogger Logger { get; }

    public Task<Result> DeleteAsync(UiPageViewModel model, bool persist = true)
        => this.DeleteAsync<UiPageViewModel, UiPage>(this._writeDbContext, model, persist, null, this.Logger);

    public async Task<UiPageViewModel?> FillViewModelAsync(UiPageViewModel? model)
    {
        if (model is null)
        {
            return null;
        }
        if (model.Id is not { } id)
        {
            throw new NullValueValidationException(nameof(model.Id));
        }
        model = await this.GetByIdAsync(id);
        return model;
    }

    public Result<Codes> GenerateCodes(in UiPageViewModel viewModel, GenerateCodesParameters? arguments = null)
    {
        _ = this.CheckValidator(viewModel);
        this.Logger.Debug($"Generating code is started.");
        var dataContextType = TypePath.New(viewModel.Dto?.Name, viewModel.Dto?.NameSpace);
        var page = new BlazorPage(viewModel.Name!)
                    .SetPageRoute(viewModel.Route)
                    .SetNameSpace(viewModel.NameSpace)
                    .SetDataContext(dataContextType);
        _ = page.Children.AddRange(viewModel.Components.Select(x => toHtmlElement(x, dataContextType, x.PageDataContextProperty is null ? null : (new TypePath(x.PageDataContextProperty.TypeFullName), x.PageDataContextProperty.Name))));

        var result = page.GenerateCodes(arguments);
        this.Logger.Debug($"Generating code is done.");

        return Result<Codes>.New(result);

        static IHtmlElement toHtmlElement(UiComponentViewModel component, string? dataContextType, (TypePath Type, string Name)? dataContextTypeProperty)
        {
            var result = BlazorComponent.New(component.Name!)
                .SetNameSpace(component.NameSpace)
                .SetDataContext(dataContextType)
                .SetDataContextProperty(dataContextTypeProperty)
                .SetPosition(component.Position.Order, component.Position.Row, component.Position.Col, component.Position.ColSpan);

            return result;
        }
    }

    public Task<IReadOnlyList<UiPageViewModel>> GetAllAsync()
        => this.GetAllAsync<UiPageViewModel, UiPage>(this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<UiPageViewModel?> GetByIdAsync(long id)
        => this.GetByIdAsync(id, this._readDbContext.UiPages
            .Include(x => x.Dto)
            .Include(x => x.Module)

            .Include(x => x.UiPageComponents)
            .ThenInclude(x => x.UiComponent)

            .Include(x => x.UiPageComponents)
            .ThenInclude(x => x.UiComponent.PageDataContext)

            .Include(x => x.UiPageComponents)
            .ThenInclude(x => x.UiComponent.PageDataContextProperty)

            .Include(x => x.UiPageComponents)
            .ThenInclude(x => x.Position), this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<Result<UiPageViewModel>> InsertAsync(UiPageViewModel model, bool persist = true)
        => this.InsertAsync(this._writeDbContext, model, this._converter.ToDbEntity, persist, onCommitted: (m, e) => m.Id = e.Id).ModelResult();

    public void ResetChanges()
        => this._writeDbContext.ResetChanges();

    public Task<Result<int>> SaveChangesAsync()
        => this._writeDbContext.SaveChangesResultAsync();

    public async Task<Result<UiPageViewModel>> UpdateAsync(long id, UiPageViewModel model, bool persist = true)
    {
        var validation = await this.ValidateAsync(model);
        if (!validation.IsSucceed)
        {
            return Result<UiPageViewModel>.From(validation, model);
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

            var save = await this.SubmitChangesAsync(persist);
            var result = (await this.GetByIdAsync(entity.Id))!;
            return Result<UiPageViewModel>.From(save, result);
        }
        finally
        {
            this.ResetChanges();
        }
    }

    public Result<UiPageViewModel?> Validate(in UiPageViewModel? model)
        => model.NotNull().Check(CheckBehavior.GatherAll)
                .NotNull(x => x.Name)
                .NotNull(x => x.NameSpace)
                .NotNull(x => x.ClassName)
                .NotNull(x => x.Dto)
                .NotNull(x => x.Module)
                .NotNull(x => x.Route).Build();

    public Task<Result<UiPageViewModel>> ValidateAsync(UiPageViewModel model)
        => this.Validate(model).ToAsync();
}