using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.BusinessServices;
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
        Result operResult;
        operResult = await this._queryService.InsertAsync(model.GetAllQueryViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await this._queryService.InsertAsync(model.GetByIdQueryViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await this._commandService.InsertAsync(model.InsertCommandViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await this._commandService.InsertAsync(model.UpdateCommandViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        operResult = await this._commandService.InsertAsync(model.DeleteCommandViewModel, false, cancellationToken);
        if (!operResult)
        {
            return operResult.WithValue(model);
        }
        var result = await ServiceHelper.InsertAsync(this, this._readDbContext, model, this._converter.ToDbEntity, false, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();
        if (persist)
        {
            _ = await this.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    public Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        ServiceHelper.UpdateAsync(this, this._readDbContext, model, this._converter.ToDbEntity, persist, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();
}