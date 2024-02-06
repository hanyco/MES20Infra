using HanyCo.Infra.Internals.Data.DataSources;

using Library.BusinessServices;
using Library.Data.Markers;
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

        var functionality = await getFunctionality(model, token);
        if (functionality == null)
        {
            return Result.CreateFailure<ObjectNotFoundException>();
        }

        await removeSegregate(functionality.GetAllQuery, token);
        await removeSegregate(functionality.GetByIdQuery, token);
        await removeSegregate(functionality.InsertCommand, token);
        await removeSegregate(functionality.UpdateCommand, token);
        await removeSegregate(functionality.DeleteCommand, token);
        await removeDto(functionality, token);
        await removeFunctionality(functionality, token);

        return Result.Success;

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel model)
            => model.Check().ArgumentNotNull().NotNull(x => x.Id);

        async Task removeSegregate(CqrsSegregate segregate, CancellationToken token)
        {
            await remove<CqrsSegregate>(segregate.Id, token);
            await remove<Dto>(segregate.ParamDtoId, token);
            await removeProperty(segregate.ParamDtoId, token);
            await remove<Dto>(segregate.ResultDtoId, token);
            await removeProperty(segregate.ResultDtoId, token);

            async Task removeProperty(long parentId, CancellationToken token)
            {
                var query = from property in this._writeDbContext.Properties
                            where property.ParentEntityId == parentId
                            select property;
                var entities = await query.ToListAsync(token);
                foreach (var p in entities)
                {
                    _ = this._writeDbContext.Remove(p);
                }
            }
        }

        async Task remove<TEntity>(long id, CancellationToken token) where TEntity : class, IIdenticalEntity
        {
            var query = from ntt in this._writeDbContext.Set<TEntity>()
                        where ntt.Id == id
                        select ntt;
            var entity = await query.FirstOrDefaultAsync(token);
            if (entity != null)
            {
                _ = this._writeDbContext.Remove(entity);
            }
        }
        
        async Task<Functionality?> getFunctionality(FunctionalityViewModel model, CancellationToken cancellationToken)
        {
            var dbQuery = from func in this._writeDbContext.Functionalities
                                    .Include(x => x.GetAllQuery)
                                    .Include(x => x.GetByIdQuery)
                                    .Include(x => x.InsertCommand)
                                    .Include(x => x.UpdateCommand)
                                    .Include(x => x.DeleteCommand)
                          where func.Id == model.Id
                          select func;
            var functionality = await dbQuery.FirstOrDefaultAsync(cancellationToken);
            return functionality;
        }

        async Task removeFunctionality(Functionality? functionality, CancellationToken token)
        {
            _ = this._writeDbContext.Functionalities.Remove(functionality!);
            _ = await this.SaveChangesAsync(token);
        }

        async Task removeDto(Functionality functionality, CancellationToken token) =>
            await remove<Dto>(functionality.SourceDtoId, token);
    }

    public Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync(CancellationToken cancellationToken = default) =>
        ServiceHelper.GetAllAsync<FunctionalityViewModel, Functionality>(this, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<FunctionalityViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        ServiceHelper.GetByIdAsync<FunctionalityViewModel, Functionality>(this, id, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public async Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        CheckPersistence(persist);
        if (!validate(model).TryParse(out var validationResult))
        {
            return validationResult;
        }

        var actionResult = await saveQueryAsync(model.GetAllQueryViewModel, model, true, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            rollback();
            return actionResult.WithValue(model);
        }
        if (cancellationToken.IsCancellationRequested)
        {
            rollback();
            return Result<FunctionalityViewModel>.CreateFailure(new TaskCanceledException(), model);
        }

        actionResult = await saveQueryAsync(model.GetByIdQueryViewModel, model, true, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            rollback();
            return actionResult.WithValue(model);
        }
        if (cancellationToken.IsCancellationRequested)
        {
            rollback();
            return Result<FunctionalityViewModel>.CreateFailure(new TaskCanceledException(), model);
        }

        actionResult = await saveCommandAsync(model.InsertCommandViewModel, model, true, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            rollback();
            return actionResult.WithValue(model);
        }
        if (cancellationToken.IsCancellationRequested)
        {
            rollback();
            return Result<FunctionalityViewModel>.CreateFailure(new TaskCanceledException(), model);
        }

        actionResult = await saveCommandAsync(model.UpdateCommandViewModel, model, true, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            rollback();
            return actionResult.WithValue(model);
        }
        if (cancellationToken.IsCancellationRequested)
        {
            rollback();
            return Result<FunctionalityViewModel>.CreateFailure(new TaskCanceledException(), model);
        }

        actionResult = await saveCommandAsync(model.DeleteCommandViewModel, model, true, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            rollback();
            return actionResult.WithValue(model);
        }
        if (cancellationToken.IsCancellationRequested)
        {
            rollback();
            return Result<FunctionalityViewModel>.CreateFailure(new TaskCanceledException(), model);
        }

        model.SourceDto.Functionality = model;
        actionResult = await this._dtoService.InsertAsync(model.SourceDto, true, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            rollback();
            return actionResult.WithValue(model);
        }
        if (cancellationToken.IsCancellationRequested)
        {
            rollback();
            return Result<FunctionalityViewModel>.CreateFailure(new TaskCanceledException(), model);
        }

        actionResult = await this.SubmitChangesAsync(true, token: cancellationToken);
        if (!actionResult.IsSucceed)
        {
            rollback();
            return actionResult.WithValue(model);
        }
        if (cancellationToken.IsCancellationRequested)
        {
            rollback();
            return Result<FunctionalityViewModel>.CreateFailure(new TaskCanceledException(), model);
        }

        await saveFunctionality(model, cancellationToken);

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
        async Task<Result> saveQueryAsync(CqrsQueryViewModel model, FunctionalityViewModel functionality, bool saveChanges, CancellationToken token)
        {
            //model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, saveChanges, token);
            result = await this._dtoService.InsertAsync(model.ResultDto, saveChanges, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._queryService.InsertAsync(model, saveChanges, token);
            if (!result.IsSucceed)
            {
                this._queryService.ResetChanges();
            }
            return result;
        }
        async Task<Result> saveCommandAsync(CqrsCommandViewModel model, FunctionalityViewModel functionality, bool persist, CancellationToken token)
        {
            //model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, persist, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._dtoService.InsertAsync(model.ResultDto, persist, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._commandService.InsertAsync(model, persist, token);
            if (!result.IsSucceed)
            {
                this._queryService.ResetChanges();
            }

            return result;
        }

        void rollback() => this._dtoService.ResetChanges();

        async Task saveFunctionality(FunctionalityViewModel model, CancellationToken cancellationToken)
        {
            var entity = this._converter.ToDbEntity(model);
            entity.Module = null;
            _ = await this._writeDbContext.Functionalities.AddAsync(entity, cancellationToken);
            _ = await this.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        if (!this.Validate(model).TryParse(out var validationCheck))
        {
            return validationCheck;
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