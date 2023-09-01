using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

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
        var validationCheck = basicChecks(model, cancellationToken)
            .ArgumentNotNull()
            .NotNull(x => x.SourceDto)
            .NotNull(x => x.GetAllQueryViewModel)
            .NotNull(x => x.GetByIdQueryViewModel)
            .NotNull(x => x.InsertCommandViewModel)
            .NotNull(x => x.UpdateCommandViewModel)
            .NotNull(x => x.DeleteCommandViewModel)
            .Build();
        if (validationCheck.IsFailure)
        {
            return validationCheck;
        }

        Result operResult;
        var saveChanges = true;
        model.SourceDto.Functionality = model;
        operResult = await this._dtoService.InsertAsync(model.SourceDto, saveChanges, cancellationToken);
        if (!operResult)
        {
            this._dtoService.ResetChanges();
            return operResult.WithValue(model);
        }

        _ = await this._dtoService.InsertAsync(model.GetAllQueryViewModel.ParamsDto, saveChanges, cancellationToken);
        _ = await this._dtoService.InsertAsync(model.GetAllQueryViewModel.ResultDto, saveChanges, cancellationToken);
        operResult = await this._queryService.InsertAsync(model.GetAllQueryViewModel, saveChanges, cancellationToken);
        if (!operResult)
        {
            this._queryService.ResetChanges();
            return operResult.WithValue(model);
        }

        _ = await this._dtoService.InsertAsync(model.GetByIdQueryViewModel.ParamsDto, saveChanges, cancellationToken);
        _ = await this._dtoService.InsertAsync(model.GetByIdQueryViewModel.ResultDto, saveChanges, cancellationToken);
        operResult = await this._queryService.InsertAsync(model.GetByIdQueryViewModel, saveChanges, cancellationToken);
        if (!operResult)
        {
            this._queryService.ResetChanges();
            return operResult.WithValue(model);
        }

        _ = await this._dtoService.InsertAsync(model.InsertCommandViewModel.ParamsDto, saveChanges, cancellationToken);
        _ = await this._dtoService.InsertAsync(model.InsertCommandViewModel.ResultDto, saveChanges, cancellationToken);
        operResult = await this._commandService.InsertAsync(model.InsertCommandViewModel, saveChanges, cancellationToken);
        if (!operResult)
        {
            this._commandService.ResetChanges();
            return operResult.WithValue(model);
        }

        _ = await this._dtoService.InsertAsync(model.UpdateCommandViewModel.ParamsDto, saveChanges, cancellationToken);
        _ = await this._dtoService.InsertAsync(model.UpdateCommandViewModel.ResultDto, saveChanges, cancellationToken);
        operResult = await this._commandService.InsertAsync(model.UpdateCommandViewModel, saveChanges, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }

        _ = await this._dtoService.InsertAsync(model.DeleteCommandViewModel.ParamsDto, saveChanges, cancellationToken);
        _ = await this._dtoService.InsertAsync(model.DeleteCommandViewModel.ResultDto, saveChanges, cancellationToken);
        operResult = await this._commandService.InsertAsync(model.DeleteCommandViewModel, saveChanges, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }

        var entity = this._converter.ToDbEntity(model);
        _ = this._writeDbContext.Functionalities.Add(entity);
        if (saveChanges)
        {
            _ = await this.SaveChangesAsync(cancellationToken).ThrowOnFailAsync();
        }

        this._dtoService.ResetChanges();
        this._queryService.ResetChanges();
        this._commandService.ResetChanges();
        this.ResetChanges();

        return operResult.WithValue(model);
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