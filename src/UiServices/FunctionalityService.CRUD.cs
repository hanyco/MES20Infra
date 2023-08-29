using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.BusinessServices;
using Library.Exceptions.Validations;
using Library.Results;

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
        var validationCheck = await this.ValidateAsync(model, cancellationToken);
        if (!validationCheck.IsSucceed)
        {
            return validationCheck.WithValue(model);
        }

        Result operResult;
        operResult = await this._dtoService.InsertAsync(model.SourceDto, false, cancellationToken);
        if (!operResult)
        {
            this._dtoService.ResetChanges();
            return operResult.WithValue(model);
        }
        //x await this.SaveChangesAsync(cancellationToken).ThrowOnFailAsync();

        //x await this._dtoService.InsertAsync(model.GetAllQueryViewModel.ParamsDto, false, cancellationToken);
        await this._dtoService.InsertAsync(model.GetAllQueryViewModel.ResultDto, false, cancellationToken);
        operResult = await this._queryService.InsertAsync(model.GetAllQueryViewModel, false, cancellationToken);
        if (!operResult)
        {
            this._queryService.ResetChanges();
            return operResult.WithValue(model);
        }
        await this.SaveChangesAsync(cancellationToken).ThrowOnFailAsync();

        await this._dtoService.InsertAsync(model.GetByIdQueryViewModel.ParamsDto, false, cancellationToken);
        await this._dtoService.InsertAsync(model.GetByIdQueryViewModel.ResultDto, false, cancellationToken);
        operResult = await this._queryService.InsertAsync(model.GetByIdQueryViewModel, false, cancellationToken);
        if (!operResult)
        {
            this._queryService.ResetChanges();
            return operResult.WithValue(model);
        }
        await this.SaveChangesAsync(cancellationToken).ThrowOnFailAsync();

        await this._dtoService.InsertAsync(model.InsertCommandViewModel.ParamsDto, false, cancellationToken);
        await this._dtoService.InsertAsync(model.InsertCommandViewModel.ResultDto, false, cancellationToken);
        operResult = await this._commandService.InsertAsync(model.InsertCommandViewModel, false, cancellationToken);
        if (!operResult)
        {
            this._commandService.ResetChanges();
            return operResult.WithValue(model);
        }
        await this.SaveChangesAsync(cancellationToken).ThrowOnFailAsync();

        await this._dtoService.InsertAsync(model.UpdateCommandViewModel.ParamsDto, false, cancellationToken);
        await this._dtoService.InsertAsync(model.UpdateCommandViewModel.ResultDto, false, cancellationToken);
        operResult = await this._commandService.InsertAsync(model.UpdateCommandViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        await this.SaveChangesAsync(cancellationToken).ThrowOnFailAsync();

        await this._dtoService.InsertAsync(model.DeleteCommandViewModel.ParamsDto, false, cancellationToken);
        await this._dtoService.InsertAsync(model.DeleteCommandViewModel.ResultDto, false, cancellationToken);
        operResult = await this._commandService.InsertAsync(model.DeleteCommandViewModel, false, cancellationToken);
        if (!operResult)
        {
            this._commandService.ResetChanges();
            return operResult.WithValue(model);
        }
        await this.SaveChangesAsync(cancellationToken).ThrowOnFailAsync();

        operResult = await ServiceHelper.InsertAsync(this, this._readDbContext, model, this._converter.ToDbEntity, false, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();
        if (!operResult)
        {
            this.ResetChanges();
            return operResult.WithValue(model);
        }
        if (persist)
        {
            operResult = await this.SaveChangesAsync(cancellationToken);
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