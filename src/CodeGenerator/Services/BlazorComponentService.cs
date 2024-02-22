using System.Collections.Immutable;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;

using Library.BusinessServices;
using Library.DesignPatterns.Markers;
using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Validations;
using Library.Windows;

using Microsoft.EntityFrameworkCore;

using Services.Helpers;

using CustomButtonViewModel = HanyCo.Infra.CodeGen.Contracts.ViewModels.UiComponentCustomButton;
using UiComponentViewModel = HanyCo.Infra.CodeGen.Contracts.ViewModels.UiComponentViewModel;
using UiPropertyViewModel = HanyCo.Infra.CodeGen.Contracts.ViewModels.UiPropertyViewModel;

namespace Services;

[Service]
[Stateless]
internal sealed class BlazorComponentService(
    InfraReadDbContext readDbContext,
    InfraWriteDbContext writeDbContext,
    IDtoService dtoService,
    IEntityViewModelConverter entityViewModelConverter) :
    IBlazorComponentService,
    IAsyncSaveChanges,
    IResetChanges,
    IService,
    IAsyncValidator<UiComponentViewModel>
{
    private readonly IEntityViewModelConverter _converter = entityViewModelConverter;
    private readonly IDtoService _dtoService = dtoService;
    private readonly InfraReadDbContext _readDbContext = readDbContext;
    private readonly InfraWriteDbContext _writeDbContext = writeDbContext;

    public UiPropertyViewModel CreateBoundPropertyByDto(DtoViewModel viewModel)
    {
        Check.MustBeArgumentNotNull(viewModel?.Name);

        return new UiPropertyViewModel
        {
            Caption = viewModel.Name,
            ControlType = ControlTypeHelper.ByDtoViewModel(viewModel),
        };
    }

    public Task<UiComponentViewModel> CreateNewComponentAsync(CancellationToken cancellationToken = default)
    {
        var result = new UiComponentViewModel();
        return Task.FromResult(result);
    }

    public async Task<UiComponentViewModel> CreateNewComponentByDtoAsync(DtoViewModel dto, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(dto?.Id);
        var entity = (await this._dtoService.GetByIdAsync(dto.Id.Value, cancellationToken)).NotNull(() => new NotFoundValidationException());
        return this.CreateViewModel(dto);
    }

    public CustomButtonViewModel CreateUnboundAction() =>
            new() { Caption = "New Action", IsEnabled = true, Placement = Placement.FormButton, Name = "NewAction", };

    public UiPropertyViewModel CreateUnboundProperty() =>
            new() { Caption = "New Property", IsEnabled = true, ControlType = ControlType.None, Name = "UnboundProperty" };

    public UiComponentViewModel CreateViewModel(DtoViewModel dto)
    {
        _ = dto.Check()
            .ArgumentNotNull().ThrowOnFail()
            .NotNull(x => x.Name)
            .NotNull(x => x.NameSpace)
            .ThrowOnFail();

        var name = CommonHelpers.Purify(dto.Name)!;
        var parsedNameSpace = CommonHelpers.Purify(dto.NameSpace);
        var result = new UiComponentViewModel
        {
            Name = name,
            PageDataContext = dto,
            ClassName = name,
            NameSpace = parsedNameSpace,
            Guid = Guid.NewGuid(),
        };
        _ = result.Properties.AddRange(dto.Properties.Compact().Select(x => this._converter.ToUiComponentProperty(x)));
        return result;
    }

    /// <summary>
    /// Deletes the asynchronous.
    /// </summary>
    /// <param name="model">The identifier.</param>
    /// <returns></returns>
    public async Task<Result> DeleteAsync(UiComponentViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        var cmpQuery = from c in this._writeDbContext.UiComponents
                       where c.Id == model.Id
                       select new
                       {
                           c.Id,
                           props = c.UiComponentProperties.Select(x => new { x.Id, x.PositionId }),
                           actions = c.UiComponentActions.Select(x => new { x.Id, x.PositionId }),
                       };
        var cmp = await cmpQuery.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        Check.MustBeNotNull(cmp, () => new NotFoundValidationException("Component not found"));
        _ = this._writeDbContext.RemoveById<UiComponent>(cmp.Id)
            .RemoveById<UiBootstrapPosition>(cmp.props.Select(x => x.PositionId))
            .RemoveById<UiBootstrapPosition>(cmp.actions.Select(x => x.PositionId));

        if (!persist)
        {
            return Result.Succeed;
        }

        try
        {
            return await this.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("infra.UiPageComponent") ?? false)
        {
            return Result.Fail(new NotificationMessage("This component is used in a page. Please remove the component from that page. Then try again.", "Unable to delete this component.", "Unable to delete"));
        }
    }

    public Task<UiPropertyViewModel?> FillUiComponentPropertyViewModelAsync(UiPropertyViewModel? prop, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(prop);
        var id = prop.Id.ArgumentNotNull(nameof(UiPropertyViewModel.Id)).Value;
        return this.GetUiComponentPropertyByIdAsync(id, cancellationToken);
    }

    public async Task<UiComponentViewModel?> FillViewModelAsync(UiComponentViewModel? model, CancellationToken cancellationToken = default)
    {
        if (model is null)
        {
            return null;
        }

        var cmpQuery = from c in this._readDbContext.UiComponents
                       .Include(x => x.PageDataContext)
                       .Include(x => x.UiComponentActions).ThenInclude(x => x.CqrsSegregate)
                       .Include(x => x.UiComponentActions).ThenInclude(x => x.Position)
                       .Include(x => x.UiComponentProperties).ThenInclude(x => x.Property)
                       .Include(x => x.UiComponentProperties).ThenInclude(x => x.Position)
                       where c.Id == model.Id
                       select c;
        var cmp = await cmpQuery.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (cmp is null)
        {
            return null;
        }

        model = this._converter.ToViewModel(cmp);
        _ = model.Properties!.AddRange(this._converter.ToViewModel(cmp.UiComponentProperties));
        _ = model.Actions!.AddRange(this._converter.ToViewModel(cmp.UiComponentActions));
        return model;
    }

    public Task<IReadOnlyList<UiComponentViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => ServiceHelper.GetAllAsync<UiComponentViewModel, UiComponent>(this, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<UiComponentViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => ServiceHelper.GetByIdAsync(this, id, this._readDbContext.UiComponents
            .Include(x => x.UiComponentActions)
            .Include(x => x.UiComponentProperties)
            .Include(x => x.UiPageComponents)
            .Include(x => x.PageDataContext)
            .Include(x => x.PageDataContextProperty), this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public async Task<IEnumerable<UiComponentViewModel>> GetByPageDataContextIdAsync(long dataDataContextId, CancellationToken cancellationToken = default)
    {
        var query = from c in this._readDbContext.UiComponents
                    where c.PageDataContextId == dataDataContextId
                    select c;
        var dbComponents = await query.ToListLockAsync(this._readDbContext.AsyncLock);
        var result = dbComponents.Select(this._converter.ToViewModel).ToImmutableArray();
        return result!;
    }

    public async Task<UiPropertyViewModel?> GetUiComponentPropertyByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var query = from cp in this._readDbContext.UiComponentProperties
                    where cp.Id == id
                    select cp;
        var result = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return this._converter.ToViewModel(result);
    }

    public Task<Result<UiComponentViewModel>> InsertAsync(UiComponentViewModel model, bool persist = true, CancellationToken cancellationToken = default)
            => ServiceHelper.InsertAsync(this, this._writeDbContext, model, this._converter.ToDbEntity, this.ValidateAsync, persist, onCommitted: (m, e) => m.Id = e.Id, cancellationToken: cancellationToken).ModelResult();

    public void ResetChanges()
        => this._writeDbContext.ResetChanges();

    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
        => this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public async Task<Result<UiComponentViewModel>> UpdateAsync(long id, UiComponentViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        Check.MustBeArgumentNotNull(model);
        model.Id = id;
        _ = await this.ValidateAsync(model, cancellationToken).ThrowOnFailAsync();
        var entity = this._converter.ToDbEntity(model)!;
        var entry = this._writeDbContext.Attach(entity);

        try
        {
            updateEntity();
            updateDataContextInfo();
            updateProperties();
            updateActions();
            return await saveChanges();
        }
        finally
        {
            _ = this._writeDbContext.Detach(entity);
        }

        void updateEntity() =>
            entry.SetModified(x => x.Caption)
                 .SetModified(x => x.IsEnabled)
                 .SetModified(x => x.Name)
                 .SetModified(x => x.IsGrid);

        void updateProperties()
        {
            //! The removed ones will be being missed if the current ones are being attached. So let's delete them all and then insert them again.
            var props = this._writeDbContext.UiComponentProperties.Where(x => x.UiComponentId == entity.Id).ToImmutableArray();
            foreach (var prop in props)
            {
                _ = this._writeDbContext.Detach(prop);
                _ = this._writeDbContext.UiComponentProperties.Remove(prop);
            }

            var newProps = entry.Entity.UiComponentProperties.ToImmutableArray();
            entry.Entity.UiComponentProperties.Clear();
            foreach (var prop in newProps)
            {
                prop.UiComponentId = entity.Id;
                entry.Entity.UiComponentProperties.Add(prop);
            }
        }

        void updateActions()
        {
            //! The removed ones will be being missed if the current ones are being attached. So let's delete them all and then insert them again.
            var actions = this._writeDbContext.UiComponentActions.Where(x => x.UiComponentId == entity.Id).ToImmutableArray();
            foreach (var action in actions)
            {
                _ = this._writeDbContext.Detach(action);
                _ = this._writeDbContext.UiComponentActions.Remove(action);
            }

            var newActions = entry.Entity.UiComponentActions.ToImmutableArray();
            entry.Entity.UiComponentActions.Clear();
            foreach (var action in newActions)
            {
                action.UiComponentId = entity.Id;
                entry.Entity.UiComponentActions.Add(action);
            }
        }

        void updateDataContextInfo()
            => entry.SetModified(x => x.PageDataContextId).SetModified(x => x.PageDataContextPropertyId);

        async Task<Result<UiComponentViewModel>> saveChanges()
        {
            var result = await this.SubmitChangesAsync(persist, token: cancellationToken);
            model.Id = entity.Id;
            return Result.From<UiComponentViewModel>(result, model);
        }
    }

    public async Task<Result<UiComponentViewModel>> ValidateAsync(UiComponentViewModel model, CancellationToken cancellationToken = default)
    {
        Check.MustBeNotNull(model?.Name);

        var nameQuery = from c in this._readDbContext.UiComponents
                        where c.Name == model.Name && c.Id != model.Id
                        select c.Id;
        var duplicate = await nameQuery.AnyAsync(cancellationToken: cancellationToken);
        var isDuplicated = Check.If(duplicate, () => new ObjectDuplicateValidationException(model.Name));
        return isDuplicated.IsSucceed ? isDuplicated.WithValue(model) : Result.Success<UiComponentViewModel>(model);
    }
}