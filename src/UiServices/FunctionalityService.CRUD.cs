using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

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

    public Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        ServiceHelper.InsertAsync(this, this._readDbContext, model, this._converter.ToDbEntity, persist, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();

    public Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        ServiceHelper.UpdateAsync(this, this._readDbContext, model, this._converter.ToDbEntity, persist, logger: this.Logger, cancellationToken: cancellationToken).ModelResult();
}