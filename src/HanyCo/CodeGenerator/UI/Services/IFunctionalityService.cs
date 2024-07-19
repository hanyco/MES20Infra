using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.UI.Services;

public interface IFunctionalityService : IService, IAsyncCrudService<FunctionalityViewModel>
{
    Task<Result<FunctionalityViewModel?>> GenerateAsync(FunctionalityViewModel args, CancellationToken token = default);
}