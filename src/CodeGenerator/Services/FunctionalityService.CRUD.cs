using HanyCo.Infra.Internals.Data.DataSources;

using Library.BusinessServices;
using Library.Exceptions;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore;

namespace Services;

internal partial class FunctionalityService
{
    public async Task<Result> DeleteAsync(FunctionalityViewModel model, bool persist = true, CancellationToken token = default)
    {
        CheckPersistence(persist);
        if (validate(model).TryParse(out var vr))
        {
            return vr;
        }

        var functionality = await getFunctionality(model.Id!.Value, token);
        if (functionality == null)
        {
            return Result.CreateFailure<ObjectNotFoundException>();
        }

        _ = await removeQuery(functionality.GetAllQueryId, token);
        _ = await removeQuery(functionality.GetByIdQueryId, token);
        _ = await removeCommand(functionality.InsertCommandId, token);
        _ = await removeCommand(functionality.UpdateCommandId, token);
        _ = await removeCommand(functionality.DeleteCommandId, token);
        _ = await removeDto(functionality.Id, token);
        _ = await removeFunctionality(functionality.SourceDtoId, token);

        return Result.Success;

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel model)
            => model.Check().ArgumentNotNull().NotNull(x => x.Id);
        async Task<Functionality?> getFunctionality(long modelId, CancellationToken token)
        {
            var dbQuery = from func in this._writeDbContext.Functionalities
                                    .Include(x => x.GetAllQuery)
                                    .Include(x => x.GetByIdQuery)
                                    .Include(x => x.InsertCommand)
                                    .Include(x => x.UpdateCommand)
                                    .Include(x => x.DeleteCommand)
                          where func.Id == modelId
                          select func;
            return await dbQuery.FirstOrDefaultAsync(token);
        }

        Task<Result> removeQuery(long queryId, CancellationToken token)
            => token.IsCancellationRequested
                ? Result.CreateFailure<TaskCanceledException>().ToAsync()
                : this._queryService.DeleteByIdAsync(queryId, token);
        Task<Result> removeCommand(long commandId, CancellationToken token)
            => token.IsCancellationRequested
                ? Result.CreateFailure<TaskCanceledException>().ToAsync()
                : this._commandService.DeleteByIdAsync(commandId, token);
        Task<Result> removeDto(long dtoId, CancellationToken token)
            => token.IsCancellationRequested
                ? Result.CreateFailure<TaskCanceledException>().ToAsync()
                : this._dtoService.DeleteByIdAsync(dtoId, token);
        async Task<Result> removeFunctionality(long functionalityId, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Result.CreateFailure<TaskCanceledException>();
            }
            else
            {
                _ = this._writeDbContext.RemoveById<Functionality>(functionalityId);
                var i = await this.SaveChangesAsync(token);
                return i > 0 ? Result.Success : Result.Failure;
            }
        }
    }

    public Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync(CancellationToken cancellationToken = default) =>
        ServiceHelper.GetAllAsync<FunctionalityViewModel, Functionality>(this, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<FunctionalityViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        ServiceHelper.GetByIdAsync<FunctionalityViewModel, Functionality>(this, id, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public async Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true, CancellationToken token = default)
    {
        CheckPersistence(persist);
        if (!validate(model).TryParse(out var validationResult))
        {
            return validationResult;
        }

        var actionResult = await saveQuery(model.GetAllQueryViewModel, token);
        if (!actionResult.IsSucceed)
        {
            this._dtoService.ResetChanges();
            return actionResult.WithValue(model);
        }

        actionResult = await saveQuery(model.GetByIdQueryViewModel, token);
        if (!actionResult.IsSucceed)
        {
            this._dtoService.ResetChanges();
            return actionResult.WithValue(model);
        }

        actionResult = await saveCommand(model.InsertCommandViewModel, token);
        if (!actionResult.IsSucceed)
        {
            this._dtoService.ResetChanges();
            return actionResult.WithValue(model);
        }

        actionResult = await saveCommand(model.UpdateCommandViewModel, token);
        if (!actionResult.IsSucceed)
        {
            this._dtoService.ResetChanges();
            return actionResult.WithValue(model);
        }

        actionResult = await saveCommand(model.DeleteCommandViewModel, token);
        if (!actionResult.IsSucceed)
        {
            this._dtoService.ResetChanges();
            return actionResult.WithValue(model);
        }

        model.SourceDto.Functionality = model;
        actionResult = await this._dtoService.InsertAsync(model.SourceDto, true, token);
        if (!actionResult.IsSucceed)
        {
            this._dtoService.ResetChanges();
            return actionResult.WithValue(model);
        }

        actionResult = await this.SubmitChangesAsync(true, token: token);
        if (!actionResult.IsSucceed)
        {
            this._dtoService.ResetChanges();
            return actionResult.WithValue(model);
        }

        _ = await saveFunctionality(model, token);

        return actionResult.WithValue(model);

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel model) =>
            BasicChecks(model)
            .NotNull(x => x!.GetAllQueryViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetAllQueryViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetAllQueryViewModel.ResultDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetByIdQueryViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetByIdQueryViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.GetByIdQueryViewModel.ResultDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.InsertCommandViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.InsertCommandViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.InsertCommandViewModel.ResultDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.UpdateCommandViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.UpdateCommandViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.UpdateCommandViewModel.ResultDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.DeleteCommandViewModel, () => "ViewModel is not initiated.")
            .NotNull(x => x!.DeleteCommandViewModel.ParamsDto, () => "ViewModel is not initiated.")
            .NotNull(x => x!.DeleteCommandViewModel.ResultDto, () => "ViewModel is not initiated.");
        async Task<Result> saveQuery(CqrsQueryViewModel model, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                this._dtoService.ResetChanges();
                return Result.CreateFailure<TaskCanceledException>();
            }
            //model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, true, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }

            result = await this._dtoService.InsertAsync(model.ResultDto, true, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }

            result = await this._queryService.InsertAsync(model, true, token);
            if (!result.IsSucceed)
            {
                this._queryService.ResetChanges();
            }
            return result;
        }
        async Task<Result> saveCommand(CqrsCommandViewModel model, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                this._dtoService.ResetChanges();
                return Result.CreateFailure<TaskCanceledException>();
            }
            //model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, true, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._dtoService.InsertAsync(model.ResultDto, true, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._commandService.InsertAsync(model, true, token);
            if (!result.IsSucceed)
            {
                this._queryService.ResetChanges();
            }

            return result;
        }
        async Task<Result> saveFunctionality(FunctionalityViewModel model, CancellationToken token)
        {
            var entity = this._converter.ToDbEntity(model);
            entity.Module = null;
            _ = await this._writeDbContext.Functionalities.AddAsync(entity, token);
            return await this.SaveChangesAsync(token);
        }
    }

    public async Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        if (!this.Validate(model).TryParse(out var validationCheck))
        {
            return validationCheck!;
        }
        if (model.SourceDto is null or { Id: null })
        {
            return Result<FunctionalityViewModel>.CreateFailure(model, () => new NullValueValidationException(nameof(model.SourceDto)))!;
        }

        Result actionResult;
        actionResult = await this._dtoService.UpdateAsync(model.SourceDto.Id.Value, model.SourceDto, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._queryService.UpdateAsync(model.GetAllQueryViewModel.Id!.Value, model.GetAllQueryViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._queryService.UpdateAsync(model.GetByIdQueryViewModel.Id!.Value, model.GetByIdQueryViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._commandService.UpdateAsync(model.InsertCommandViewModel.Id!.Value, model.InsertCommandViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._commandService.UpdateAsync(model.UpdateCommandViewModel.Id!.Value, model.UpdateCommandViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await this._commandService.UpdateAsync(model.DeleteCommandViewModel.Id!.Value, model.DeleteCommandViewModel, false, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        var result = await ServiceHelper.UpdateAsync(this, this._readDbContext, model, this._converter.ToDbEntity, false, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();
        if (persist)
        {
            _ = await this.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    private static void CheckPersistence(bool persist)
    {
        if (!persist)
        {
            throw new NotSupportedException("non-persistent operation is not supported.");
        }
    }
}