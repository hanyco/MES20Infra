using System.Diagnostics.CodeAnalysis;

using Contracts.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace Contracts.Services;

public interface IFunctionalityService : IBusinesService, IAsyncCrudService<FunctionalityViewModel>
{
    Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync([DisallowNull] FunctionalityViewModel model, CancellationToken token = default);
}