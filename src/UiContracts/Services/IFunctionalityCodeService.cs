using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace Contracts.Services;

public interface IFunctionalityCodeService : IBusinessService, IAsyncCodeGenerator<FunctionalityViewModel>
{

}