﻿using HanyCo.Infra.Internals.Data.DataSources;

using Library.BusinessServices;
using Library.Exceptions.Validations;
using Library.Results;
using Library.Validations;

namespace Services;

internal partial class FunctionalityService
{
    public Task<Result> DeleteAsync(FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        ServiceHelper.DeleteAsync<FunctionalityViewModel, Functionality>(this, this._writeDbContext, model, persist, persist, this.Logger);

    public Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync(CancellationToken cancellationToken = default) =>
        ServiceHelper.GetAllAsync<FunctionalityViewModel, Functionality>(this, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<FunctionalityViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        ServiceHelper.GetByIdAsync<FunctionalityViewModel, Functionality>(this, id, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public async Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        var saveChanges = true;

        if (!validate(model, cancellationToken).TryParse(out var validationResult))
        {
            return validationResult;
        }

        Result actionResult;

        model.SourceDto.Functionality = model;

        actionResult = await this._dtoService.InsertAsync(model.SourceDto, saveChanges, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            this._dtoService.ResetChanges();
            return actionResult.WithValue(model);
        }
        actionResult = await saveQueryAsync(model.GetAllQueryViewModel, model, saveChanges, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await saveQueryAsync(model.GetByIdQueryViewModel, model, saveChanges, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await saveCommandAsync(model.InsertCommandViewModel, model, saveChanges, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await saveCommandAsync(model.UpdateCommandViewModel, model, saveChanges, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }
        actionResult = await saveCommandAsync(model.DeleteCommandViewModel, model, saveChanges, cancellationToken);
        if (!actionResult.IsSucceed)
        {
            return actionResult.WithValue(model);
        }

        var entity = this._converter.ToDbEntity(model);
        _ = this._writeDbContext.Functionalities.Add(entity);
        if (saveChanges)
        {
            actionResult = await this.SubmitChangesAsync(saveChanges, token: cancellationToken);
        }

        return actionResult.WithValue(model);

        static Result<FunctionalityViewModel> validate(FunctionalityViewModel model, CancellationToken cancellationToken) =>
            BasicChecks(model)
                    .NotNull(x => x.SourceDto)
                    .NotNull(x => x.GetAllQueryViewModel)
                    .NotNull(x => x.GetAllQueryViewModel.ParamsDto)
                    .NotNull(x => x.GetAllQueryViewModel.ResultDto)
                    .NotNull(x => x.GetByIdQueryViewModel)
                    .NotNull(x => x.GetByIdQueryViewModel.ParamsDto)
                    .NotNull(x => x.GetByIdQueryViewModel.ResultDto)
                    .NotNull(x => x.InsertCommandViewModel)
                    .NotNull(x => x.InsertCommandViewModel.ParamsDto)
                    .NotNull(x => x.InsertCommandViewModel.ResultDto)
                    .NotNull(x => x.UpdateCommandViewModel)
                    .NotNull(x => x.UpdateCommandViewModel.ParamsDto)
                    .NotNull(x => x.UpdateCommandViewModel.ResultDto)
                    .NotNull(x => x.DeleteCommandViewModel)
                    .NotNull(x => x.DeleteCommandViewModel.ParamsDto)
                    .NotNull(x => x.DeleteCommandViewModel.ResultDto);
        async Task<Result> saveQueryAsync(CqrsQueryViewModel model, FunctionalityViewModel functionality, bool saveChanges, CancellationToken token)
        {
            //! Not used. one-way to UI.
            //x model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, saveChanges, token);
            if (!result.IsSucceed)
            {
                return result;
                this._dtoService.ResetChanges();
            }
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
        async Task<Result> saveCommandAsync(CqrsCommandViewModel model, FunctionalityViewModel functionality, bool saveChanges, CancellationToken token)
        {
            model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, saveChanges, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._dtoService.InsertAsync(model.ResultDto, saveChanges, token);
            if (!result.IsSucceed)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._commandService.InsertAsync(model, saveChanges, token);
            if (!result.IsSucceed)
            {
                this._queryService.ResetChanges();
            }

            return result;
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
}