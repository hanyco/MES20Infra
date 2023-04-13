using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Results;
using Library.Validations;

using Microsoft.EntityFrameworkCore.Storage;

namespace Services;

partial class FunctionalityService
{
    Task<IDbContextTransaction> IAsyncTransactionalService.BeginTransactionAsync(CancellationToken cancellationToken)
        => this._writeDbContext.BeginTransactionAsync(cancellationToken);

    Task<Result> IAsyncTransactionalService.CommitTransactionAsync(CancellationToken cancellationToken)
        => this._writeDbContext.CommitTransactionAsync(cancellationToken);

    public void ResetChanges()
        => this._writeDbContext.ResetChanges();

    Task IAsyncTransactionalService.RollbackTransactionAsync(CancellationToken cancellationToken)
        => this._writeDbContext.Database.RollbackTransactionAsync(cancellationToken);

    public Task<Result<int>> SaveChangesAsync()
        => this._writeDbContext.SaveChangesResultAsync();

    Task<Result<FunctionalityViewModel>> IAsyncValidator<FunctionalityViewModel>.ValidateAsync(FunctionalityViewModel viewModel)
        => viewModel.Check()
            .ArgumentNotNull()
            .NotNull(x => x.Name)
            .NotNull(x => x.NameSpace)
            .Build().ToAsync();
}