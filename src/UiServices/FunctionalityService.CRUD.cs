using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;
using HanyCo.Infra.UI.ViewModels;

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
        const bool SAVE_CHANGES = true;

        var validationRoles = getValidations(model, cancellationToken);
        if (!validationRoles.Build().TryParse(out var validationChecks))
        {
            return validationChecks;
        }

        Result operResult;
        model.SourceDto.Functionality = model;
        operResult = await this._dtoService.InsertAsync(model.SourceDto, SAVE_CHANGES, cancellationToken);
        if (!operResult)
        {
            this._dtoService.ResetChanges();
            return operResult.WithValue(model);
        }
        operResult = await saveQueryAsync(model.GetAllQueryViewModel, model, SAVE_CHANGES, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await saveQueryAsync(model.GetByIdQueryViewModel, model, SAVE_CHANGES, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await saveCommandAsync(model.InsertCommandViewModel, model, SAVE_CHANGES, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await saveCommandAsync(model.UpdateCommandViewModel, model, SAVE_CHANGES, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await saveCommandAsync(model.DeleteCommandViewModel, model, SAVE_CHANGES, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }

        var entity = this._converter.ToDbEntity(model);
        _ = this._writeDbContext.Functionalities.Add(entity);
        if (SAVE_CHANGES)
        {
            operResult = await this.SubmitChangesAsync(SAVE_CHANGES, token: cancellationToken);
        }

        return operResult.WithValue(model);

        static ValidationResultSet<FunctionalityViewModel> getValidations(FunctionalityViewModel model, CancellationToken cancellationToken) =>
            BasicChecks(model, cancellationToken)
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
            model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, saveChanges, token);
            if (!result)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._dtoService.InsertAsync(model.ResultDto, saveChanges, token);
            if (!result)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._queryService.InsertAsync(model, saveChanges, token);
            if (!result)
            {
                this._queryService.ResetChanges();
            }

            return result;
        }
        async Task<Result> saveCommandAsync(CqrsCommandViewModel model, FunctionalityViewModel functionality, bool saveChanges, CancellationToken token)
        {
            model.ParamsDto.Functionality = model.ResultDto.Functionality = functionality;
            Result result = await this._dtoService.InsertAsync(model.ParamsDto, saveChanges, token);
            if (!result)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._dtoService.InsertAsync(model.ResultDto, saveChanges, token);
            if (!result)
            {
                this._dtoService.ResetChanges();
                return result;
            }
            result = await this._commandService.InsertAsync(model, saveChanges, token);
            if (!result)
            {
                this._queryService.ResetChanges();
            }

            return result;
        }
    }

    public async Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default)
    {
        var validationCheck = await this.ValidateAsync(model, cancellationToken);
        if (validationCheck.IsSucceed)
        {
            return validationCheck.WithValue(model);
        }
        if (model.SourceDto is null or { Id: null })
        {
            return Result<FunctionalityViewModel>.CreateFailure(model, () => new NullValueValidationException(nameof(model.SourceDto)))!;
        }

        Result operResult;
        operResult = await this._dtoService.UpdateAsync(model.SourceDto.Id.Value, model.SourceDto, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await this._queryService.UpdateAsync(model.GetAllQueryViewModel.Id.Value, model.GetAllQueryViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await this._queryService.UpdateAsync(model.GetByIdQueryViewModel.Id.Value, model.GetByIdQueryViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await this._commandService.UpdateAsync(model.InsertCommandViewModel.Id.Value, model.InsertCommandViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await this._commandService.UpdateAsync(model.UpdateCommandViewModel.Id.Value, model.UpdateCommandViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await this._commandService.UpdateAsync(model.DeleteCommandViewModel.Id.Value, model.DeleteCommandViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        var result = await ServiceHelper.UpdateAsync(this, this._readDbContext, model, this._converter.ToDbEntity, false, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();
        if (persist)
        {
            _ = await this.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}