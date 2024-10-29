using HanyCo.Infra.CodeGen.Domain.Services;
using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace Services;

internal partial class ControllerService : IControllerApiService
{
    public Task<Result<int>> DeleteAsync(ControllerMethodViewModel model, bool persist = true, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    Task<IReadOnlyList<ControllerMethodViewModel>> IAsyncRead<ControllerMethodViewModel, long>.GetAllAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    Task<ControllerMethodViewModel?> IAsyncRead<ControllerMethodViewModel, long>.GetByIdAsync(long id, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Result<ControllerMethodViewModel>> InsertAsync(ControllerMethodViewModel model, bool persist = true, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<Result<ControllerMethodViewModel>> UpdateAsync(long id, ControllerMethodViewModel model, bool persist = true, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
