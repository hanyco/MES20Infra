using HanyCo.Infra.UI.ViewModels;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.UI.Services;

public interface IFunctionalityService : IBusinesService, IAsyncCrudService<FunctionalityViewModel>
{
    Task<Result<FunctionalityViewModel?>> GenerateAsync(FunctionalityViewModel model, CancellationToken token = default);
}