using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace Contracts.Services;

public interface IFunctionalityService : IBusinesService, IAsyncCrudService<FunctionalityViewModel>
{
    Task<Result<FunctionalityViewModel?>> GenerateAsync([DisallowNull] FunctionalityViewModel model, CancellationToken token = default);
}