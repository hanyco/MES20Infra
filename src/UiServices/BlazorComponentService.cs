using System.Collections.Immutable;

using Contracts.Services;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.Markers;
using HanyCo.Infra.UI.ViewModels;

using Library.Exceptions.Validations;
using Library.Interfaces;
using Library.Results;
using Library.Validations;
using Library.Windows;

using Microsoft.EntityFrameworkCore;

namespace Services;

[Service]
public sealed class BlazorComponentService :
    IBlazorComponentService,
    IAsyncValidator<UiComponentViewModel>,
    IAsyncSaveChanges,
    IResetChanges,
    IService
{
    private readonly IEntityViewModelConverter _converter;
    private readonly InfraReadDbContext _readDbContext;
    private readonly InfraWriteDbContext _writeDbContext;

    public BlazorComponentService(InfraReadDbContext readDbContext,
                    InfraWriteDbContext writeDbContext,
        IEntityViewModelConverter entityViewModelConverter)
    {
        this._readDbContext = readDbContext;
        this._writeDbContext = writeDbContext;
        this._converter = entityViewModelConverter;
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
        Check.NotNull(cmp, () => new NotFoundValidationException("Component not found"));
        _ = this._writeDbContext.RemoveById<UiComponent>(cmp.Id)
            .RemoveById<UiBootstrapPosition>(cmp.props.Select(x => x.PositionId))
            .RemoveById<UiBootstrapPosition>(cmp.actions.Select(x => x.PositionId));

        if (!persist)
        {
            return Result.Success;
        }

        try
        {
            return await this.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("infra.UiPageComponent") ?? false)
        {
            return Result.CreateFailure(new NotificationMessage("This component is used in a page. Please remove the component from that page. Then try again.", "Unable to delete this component.", "Unable to delete"));
        }
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
        model = this._converter.ToViewModel(cmp)!;
        _ = model?.UiProperties?.AddRange(this._converter.ToViewModel(cmp?.UiComponentProperties)!);
        _ = model?.UiActions?.AddRange(this._converter.ToViewModel(cmp?.UiComponentActions)!);
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

    public Task<Result<UiComponentViewModel>> InsertAsync(UiComponentViewModel model, bool persist = true, CancellationToken cancellationToken = default)
        => ServiceHelper.InsertAsync(this, this._writeDbContext, model, this._converter.ToDbEntity, this.ValidateAsync, persist, onCommitted: (m, e) => m.Id = e.Id, cancellationToken: cancellationToken).ModelResult();

    public void ResetChanges()
        => this._writeDbContext.ResetChanges();

    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
        => this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public async Task<Result<UiComponentViewModel>> UpdateAsync(long id, UiComponentViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        Check.IfArgumentNotNull(model);
        model.Id = id;
        _ = await this.CheckValidatorAsync(model);
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
                 .SetModified(x => x.Name);

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
            var result = await this.SubmitChangesAsync(persist);
            model.Id = entity.Id;
            return Result<UiComponentViewModel>.From(result, model);
        }
    }

    public async Task<Result<UiComponentViewModel>> ValidateAsync(UiComponentViewModel model, CancellationToken cancellationToken = default)
    {
        Check.NotNull(model?.Name);

        var nameQuery = from c in this._readDbContext.UiComponents
                        where c.Name == model.Name && c.Id != model.Id
                        select c.Id;
        Check.If(await nameQuery.AnyAsync(cancellationToken: cancellationToken), () => new ObjectDuplicateValidationException(nameof(model.Name)));
        return new(model);
    }
}