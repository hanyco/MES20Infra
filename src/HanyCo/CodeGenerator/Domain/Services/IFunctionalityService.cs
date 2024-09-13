using System.Diagnostics.CodeAnalysis;

using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IFunctionalityService : IBusinessService, IAsyncCrud<FunctionalityViewModel>, IAsyncCreator<FunctionalityViewModel>
{
    Task<Result<FunctionalityViewModel?>> GenerateViewModelAsync([DisallowNull] FunctionalityViewModel model, CancellationToken token = default);
}