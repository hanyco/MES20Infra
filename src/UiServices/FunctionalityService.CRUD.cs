using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.Results;

namespace Services;

internal partial class FunctionalityService
{
    #region CRUD

    public Task<Result> DeleteAsync(FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken)
        => ServiceHelper.DeleteAsync<FunctionalityViewModel, Functionality>(this, this._writeDbContext, model, persist, persist, this.Logger);

    public Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync(CancellationToken cancellationToken)
        => ServiceHelper.GetAllAsync<FunctionalityViewModel, Functionality>(this, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<FunctionalityViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken)
        => ServiceHelper.GetByIdAsync<FunctionalityViewModel, Functionality>(this, id, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken)
        => ServiceHelper.InsertAsync(this, this._readDbContext, model, this._converter.ToDbEntity, persist, logger: this.Logger).ModelResult();

    public Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken)
        => ServiceHelper.UpdateAsync(this, this._readDbContext, model, this._converter.ToDbEntity, persist, logger: this.Logger).ModelResult();

    #endregion CRUD
}