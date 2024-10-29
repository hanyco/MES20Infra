using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IControllerService : IBusinessService, ICodeGenerator<ControllerViewModel>, 
    IAsyncCrud<ControllerViewModel>,
    IAsyncCrud<ControllerMethodViewModel>
{
    Task<Result> DeleteById(long controllerId, bool persist = true, CancellationToken token = default);
}
