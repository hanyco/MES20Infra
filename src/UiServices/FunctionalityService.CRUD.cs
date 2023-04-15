using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Contracts.ViewModels;

using HanyCo.Infra.Internals.Data.DataSources;

using Library.Results;

namespace Services;
partial class FunctionalityService
{
    #region CRUD

    public Task<Result> DeleteAsync(FunctionalityViewModel model, bool persist = true)
        => this.DeleteAsync<FunctionalityViewModel, Functionality>(this._writeDbContext, model, persist, persist, this.Logger);

    public Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync()
        => this.GetAllAsync<FunctionalityViewModel, Functionality>(this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<FunctionalityViewModel?> GetByIdAsync(long id)
        => this.GetByIdAsync<FunctionalityViewModel, Functionality>(id, this._readDbContext, this._converter.ToViewModel, this._readDbContext.AsyncLock);

    public Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true)
        => this.InsertAsync(this._readDbContext, model, this._converter.ToDbEntity, persist, logger: this.Logger).ModelResult();

    public Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true)
        => this.UpdateAsync(this._readDbContext, model, this._converter.ToDbEntity, persist, logger: this.Logger).ModelResult();

    #endregion CRUD
}
