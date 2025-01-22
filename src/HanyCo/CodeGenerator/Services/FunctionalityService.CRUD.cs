using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Data.EntityFrameworkCore;
using Library.Exceptions;
using Library.Interfaces;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal partial class FunctionalityService : IValidator<FunctionalityViewModel>, IAsyncTransactionalSave
{
    public async Task<Result<int>> DeleteAsync(FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        CheckPersistence(persist);
        if (!validate(model).TryParse(out var vr))
        {
            return vr.WithValue(-1);
        }

        var functionality = await getFunctionality(model.Id!.Value, cancellationToken);
        if (functionality == null)
        {
            return Result.Fail<int, ObjectNotFoundException>();
        }

        var tasks = new Collection<Func<Functionality, Task<Result>>>
        {
            x => removeFunctionality(x, cancellationToken),
            x => removeDto(x.SourceDto, cancellationToken),
            x => removeQuery(x.GetAllQuery, cancellationToken),
            x => removeQuery(x.GetByIdQuery, cancellationToken),
            x => removeCommand(x.InsertCommand, cancellationToken),
            x => removeCommand(x.UpdateCommand, cancellationToken),
            x => removeCommand(x.DeleteCommand, cancellationToken),
            x => removeController(x.Controller, cancellationToken)
        };

        var removeEntitiesResult = await tasks.RunAllAsync(functionality, cancellationToken).ThrowIfCancellationRequested(cancellationToken);
        if (removeEntitiesResult.IsFailure)
        {
            return removeEntitiesResult.WithValue(0);
        }

        var saveResult = await this.SaveChangesAsync(cancellationToken);
        return saveResult;

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel model) =>
            model.Check().ArgumentNotNull().NotNull(x => x.Id);
        Task<Functionality?> getFunctionality(long modelId, CancellationToken token) =>
            this.GetByIdFunctionality(modelId, token);
        Task<Result> removeQuery(CqrsSegregate query, CancellationToken token) =>
            removeSegregate(query, this._queryService.DeleteById, token);
        Task<Result> removeCommand(CqrsSegregate command, CancellationToken token) =>
            removeSegregate(command, this._commandService.DeleteById, token);
        Task<Result> removeDto(Dto dto, CancellationToken token) =>
            dto is null ? Task.FromResult(Result.Succeed) : this._dtoService.DeleteById(dto.Id, true, token);
        Task<Result> removeController(Controller? controller, CancellationToken token) =>
            controller is null ? Task.FromResult(Result.Succeed) : this._controllerService.DeleteById(controller.Id, true, token);
        Task<Result> removeFunctionality(Functionality functionality, CancellationToken token)
        {
            _ = this._writeDbContext.RemoveById<Functionality>(functionality.Id);
            return Task.FromResult(Result.Succeed);
        }
        async Task<Result> removeSegregate(CqrsSegregate segregate, Func<long, bool, CancellationToken, Task<Result>> deleteByIdAsync, CancellationToken token)
        {
            var result = await deleteByIdAsync(segregate.Id, true, token);
            if (result.IsFailure)
            {
                return result;
            }
            result = await removeDto(segregate.ParamDto, token);
            if (result.IsFailure)
            {
                return result;
            }

            result = await removeDto(segregate.ResultDto, token);
            return result;
        }
    }

    public Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync(CancellationToken cancellationToken = default) =>
        this.GetAllAsync<FunctionalityViewModel, Functionality>(this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public async Task<FunctionalityViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        // Read from database
        var query = from f in this._readDbContext.Functionalities.Include(f => f.Module)
                    where f.Id == id
                    select f;
        var dbResult = await query.FirstOrDefaultLockAsync(this._readDbContext.AsyncLock, cancellationToken).ThrowIfCancellationRequested(cancellationToken);
        if (dbResult == null)
        {
            return null;
        }

        var result = this._converter.ToViewModel(dbResult);
        result.SourceDto = await this._dtoService.GetByIdAsync(dbResult.SourceDtoId, cancellationToken).ThrowIfCancellationRequested(cancellationToken);
        result.GetAllQuery = await this._queryService.GetByIdAsync(dbResult.GetAllQueryId, cancellationToken).ThrowIfCancellationRequested(cancellationToken);
        result.GetByIdQuery = await this._queryService.GetByIdAsync(dbResult.GetByIdQueryId, cancellationToken).ThrowIfCancellationRequested(cancellationToken);
        result.InsertCommand = await this._commandService.GetByIdAsync(dbResult.InsertCommandId, cancellationToken).ThrowIfCancellationRequested(cancellationToken);
        result.UpdateCommand = await this._commandService.GetByIdAsync(dbResult.UpdateCommandId, cancellationToken).ThrowIfCancellationRequested(cancellationToken);
        result.DeleteCommand = await this._commandService.GetByIdAsync(dbResult.DeleteCommandId, cancellationToken).ThrowIfCancellationRequested(cancellationToken);
        result.Controller = await this._controllerService.GetByIdAsync(dbResult.ControllerId, cancellationToken).ThrowIfCancellationRequested(cancellationToken);

        var (_, (data, _)) = InitializeWorkspace(result, cancellationToken);
        await this.CreateBlazorListPage(data, cancellationToken);
        await this.CreateBlazorDetailsPage(data, cancellationToken);
        await this.CreateBlazorListComponent(data, cancellationToken);
        await this.CreateBlazorDetailsComponent(data, cancellationToken);
        return result;
    }

    public async Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true, CancellationToken token = default)
    {
        // Non-persistent operation is not supported.
        if (!CatchResult(() => CheckPersistence(persist)).TryParse(out var persistenceCheckResult))
        {
            return persistenceCheckResult.WithValue(model);
        }
        // Validate the model
        if (!this.Validate(model).TryParse(out var vr))
        {
            return vr!;
        }
        // Check if the functionality or it's components already exist.
        var er = await checkExitance(model);
        if (er.IsFailure)
        {
            return er.WithValue(model);
        }

        // Save the functionality components
        using var transaction = await this._writeDbContext.BeginTransactionAsync(token);
        try
        {
            await saveQuery(model.GetAllQuery, token).ThrowIfCancellationRequested(token);
            await saveQuery(model.GetByIdQuery, token).ThrowIfCancellationRequested(token);
            await saveCommand(model.InsertCommand, token).ThrowIfCancellationRequested(token);
            await saveCommand(model.UpdateCommand, token).ThrowIfCancellationRequested(token);
            await saveCommand(model.DeleteCommand, token).ThrowIfCancellationRequested(token);
            await saveDto(model.SourceDto, token).ThrowIfCancellationRequested(token);
            await saveController(model, token).ThrowIfCancellationRequested(token);
            await saveFunctionality(model, token).ThrowIfCancellationRequested(token);

            var saveResult = await this.SubmitChangesAsync(persist, transaction, token: token).OnFailure(this._dtoService.ResetChanges).ThrowOnFailAsync(cancellationToken: token);
            return Result.Success(model);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(token);

            return Result.Fail<FunctionalityViewModel>(ex);
        }

        async Task saveQuery(CqrsQueryViewModel model, CancellationToken token)
        {
            _ = await this._dtoService.InsertAsync(model.ParamsDto, true, token)
                .OnFailure(this._dtoService.ResetChanges)
                .ThrowOnFailAsync(cancellationToken: token);

            _ = await this._dtoService.InsertAsync(model.ResultDto, true, token)
                .OnFailure(this._dtoService.ResetChanges)
                .ThrowOnFailAsync(cancellationToken: token);

            _ = await this._queryService.InsertAsync(model, true, token)
                .OnFailure(this._queryService.ResetChanges)
                .ThrowOnFailAsync(cancellationToken: token);
        }
        async Task saveCommand(CqrsCommandViewModel model, CancellationToken token)
        {
            _ = await this._dtoService.InsertAsync(model.ParamsDto, true, token)
                .OnFailure(this._dtoService.ResetChanges)
                .ThrowOnFailAsync(cancellationToken: token);

            _ = await this._dtoService.InsertAsync(model.ResultDto, true, token)
                .OnFailure(this._dtoService.ResetChanges)
                .ThrowOnFailAsync(cancellationToken: token);

            _ = await this._commandService.InsertAsync(model, true, token)
                .OnFailure(this._commandService.ResetChanges)
                .ThrowOnFailAsync(cancellationToken: token);
        }
        Task saveDto(DtoViewModel model, CancellationToken token) =>
            this._dtoService.InsertAsync(model, true, token)
                .OnFailure(this._dtoService.ResetChanges)
                .ThrowOnFailAsync(cancellationToken: token);

        async Task saveFunctionality(FunctionalityViewModel model, CancellationToken token)
        {
            //Reset the navigation properties
            var entity = this._converter.ToDbEntity(model);
            var getAllId = entity.GetAllQuery.Id;
            var getById = entity.GetByIdQuery.Id;
            var insertId = entity.InsertCommand.Id;
            var updateId = entity.UpdateCommand.Id;
            var deleteId = entity.DeleteCommand.Id;
            var moduleId = entity.Module?.Id ?? entity.ModuleId;
            var sourceDtoId = entity.SourceDto.Id;

            entity.GetAllQuery = entity.GetByIdQuery = entity.InsertCommand = entity.UpdateCommand = entity.DeleteCommand = null!;
            entity.Module = null!;
            entity.SourceDto = null!;
            entity.Controller = null!;

            entity.GetAllQueryId = getAllId;
            entity.GetByIdQueryId = getById;
            entity.InsertCommandId = insertId;
            entity.UpdateCommandId = updateId;
            entity.DeleteCommandId = deleteId;
            entity.ModuleId = moduleId;
            entity.SourceDtoId = sourceDtoId;

            var entry = await this._writeDbContext.Functionalities.AddAsync(entity, token);
            entity.Module = null!;
            _ = await this.SaveChangesAsync(token)
                .OnFailure(this._dtoService.ResetChanges)
                .ThrowOnFailAsync(cancellationToken: token);
        }

        async Task<Result> checkExitance(FunctionalityViewModel model)
        {
            var exists = await this.AnyByNameAsync(model.Name!);
            if (exists)
            {
                return Result.Fail("Functionality already exists.");
            }
            exists = await this._dtoService.AnyByNameAsync(model.SourceDto.Name!);
            if (exists)
            {
                return Result.Fail("Source DTO already exists.");
            }
            exists = await this._queryService.AnyByNameAsync(model.GetAllQuery.Name!);
            if (exists)
            {
                return Result.Fail("GetAll query already exists.");
            }
            exists = await this._queryService.AnyByNameAsync(model.GetByIdQuery.Name!);
            if (exists)
            {
                return Result.Fail("GetById query already exists.");
            }
            exists = await this._commandService.AnyByName(model.InsertCommand.Name!);
            if (exists)
            {
                return Result.Fail("Insert command already exists.");
            }
            exists = await this._commandService.AnyByName(model.UpdateCommand.Name!);
            if (exists)
            {
                return Result.Fail("Update command already exists.");
            }
            exists = await this._commandService.AnyByName(model.DeleteCommand.Name!);
            if (exists)
            {
                return Result.Fail("Delete command already exists.");
            };
            return Result.Succeed;
        }

        async Task saveController(FunctionalityViewModel model, CancellationToken token) =>
            await this._controllerService.SaveViewModelAsync(model.Controller, cancellationToken: token)
                .OnFailure(this._dtoService.ResetChanges)
                .ThrowOnFailAsync(cancellationToken: token);
    }

    public async Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        CheckPersistence(persist);
        if (!this.Validate(model).TryParse(out var validationCheck))
        {
            return validationCheck!;
        }
        if (model.SourceDto is null or { Id: null })
        {
            return Result.Fail(model)!;
        }

        Result actionResult;
        actionResult = await this._dtoService.UpdateAsync(model.SourceDto.Id.Value, model.SourceDto, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._queryService.UpdateAsync(model.GetAllQuery.Id!.Value, model.GetAllQuery, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._queryService.UpdateAsync(model.GetByIdQuery.Id!.Value, model.GetByIdQuery, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._commandService.UpdateAsync(model.InsertCommand.Id!.Value, model.InsertCommand, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._commandService.UpdateAsync(model.UpdateCommand.Id!.Value, model.UpdateCommand, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._commandService.UpdateAsync(model.DeleteCommand.Id!.Value, model.DeleteCommand, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._controllerService.UpdateAsync(model.Controller.Id!.Value, model.Controller, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        var result = await DataServiceHelper.UpdateAsync(this, this._readDbContext, model, this._converter.ToDbEntity, false, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();
        if (persist)
        {
            _ = await this.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    private static void CheckPersistence([DoesNotReturnIf(false)] bool persist) =>
        Check.MustBe(persist, () => new NotSupportedException("non-persistent operation is not supported."));

    private Task<bool> AnyByNameAsync(string name)
    {
        var query = from dto in this._readDbContext.Functionalities
                    where dto.Name == name
                    select dto.Id;
        return query.AnyLockAsync(this._readDbContext.AsyncLock);
    }

    private async Task<Functionality?> GetByIdFunctionality(long id, CancellationToken cancellationToken)
    {
        var funcQuery = from func in this._readDbContext.Functionalities
                            .Include(f => f.DeleteCommand)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.DeleteCommand)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.DeleteCommand)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.GetAllQuery)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.GetAllQuery)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.GetAllQuery)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.GetByIdQuery)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.GetByIdQuery)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.GetByIdQuery)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.InsertCommand)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.InsertCommand)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.InsertCommand)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.UpdateCommand)
                                .ThenInclude(cs => cs.Module)
                            .Include(f => f.UpdateCommand)
                                .ThenInclude(cs => cs.ParamDto)
                            .Include(f => f.UpdateCommand)
                                .ThenInclude(cs => cs.ResultDto)
                            .Include(f => f.Module)
                            .Include(f => f.SourceDto)
                                .ThenInclude(f => f.Module)
                        where func.Id == id
                        select func;
        var functionality = await funcQuery.FirstOrDefaultAsync(cancellationToken);
        if (functionality == null)
        {
            return null;
        }

        await fillProps(functionality.GetAllQuery);
        await fillProps(functionality.GetByIdQuery);
        await fillProps(functionality.InsertCommand);
        await fillProps(functionality.UpdateCommand);
        await fillProps(functionality.DeleteCommand);
        await fillProperties(functionality.SourceDto.Properties, functionality.SourceDtoId);

        return functionality;

        async Task fillProps(CqrsSegregate cqrs)
        {
            await fillProperties(cqrs.ParamDto.Properties, cqrs.ParamDtoId);
            await fillProperties(cqrs.ResultDto.Properties, cqrs.ResultDtoId);
        }

        async Task fillProperties(ICollection<Property> properties, long paramDtoId)
            => properties.AddRange(await getProperties(paramDtoId));

        async Task<List<Property>> getProperties(long parentEntityId)
        {
            var propertiesQuery = from x in this._readDbContext.Properties
                                  where x.ParentEntityId == parentEntityId
                                  select x;
            var properties = await propertiesQuery.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
            return properties;
        }
    }
}