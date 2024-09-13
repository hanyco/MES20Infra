using System.Windows.Navigation;

using Library.Results;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Services;

internal partial class FunctionalityService
{
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        this._writeDbContext.BeginTransactionAsync(cancellationToken);

    public Task<Result> CommitTransactionAsync(CancellationToken cancellationToken = default) =>
        this._writeDbContext.CommitTransactionAsync(cancellationToken);

    public async Task<FunctionalityViewModel> CreateAsync(CancellationToken token = default) =>
        new FunctionalityViewModel
        {
            SourceDto = await this._dtoService.CreateAsync(token)
        };

    public Task<Result<int>> DeleteAsync(FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public async Task<IReadOnlyList<FunctionalityViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = from func in _readDbContext.Functionalities
                       select func;
        var dbResult = await query.ToListLockAsync(_readDbContext.AsyncLock, cancellationToken);
        var result = this._converter.ToViewModel(dbResult);
        return result.ToList()!;
    }

    public Task<FunctionalityViewModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<Result<FunctionalityViewModel>> InsertAsync(FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public void ResetChanges() =>
        this._writeDbContext.ResetChanges();

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) =>
        this._writeDbContext.Database.RollbackTransactionAsync(cancellationToken);

    public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        this._writeDbContext.SaveChangesResultAsync(cancellationToken: cancellationToken);

    public Task<Result<FunctionalityViewModel>> UpdateAsync(long id, FunctionalityViewModel model, bool persist = true, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}