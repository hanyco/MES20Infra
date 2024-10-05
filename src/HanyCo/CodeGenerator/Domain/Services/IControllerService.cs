using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IControllerService : IBusinessService, ICodeGenerator<ControllerViewModel>, IAsyncCrud<ControllerViewModel>
{
}