using System.Diagnostics.CodeAnalysis;

using Contracts.ViewModels;

using Library.Interfaces;
using Library.Results;
using Library.Wpf.Windows;

namespace Contracts.Services;

public interface IFunctionalityService : IBusinessService, IAsyncCrud<FunctionalityViewModel>, IAsyncCreator<FunctionalityViewModel>
{
    Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync([DisallowNull] FunctionalityViewModel model, CancellationToken token = default);
}