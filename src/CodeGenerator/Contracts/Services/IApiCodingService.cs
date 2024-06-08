using HanyCo.Infra.CodeGen.Contracts.CodeGen.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IApiCodingService : IBusinessService, ICodeGenerator<ApiCodingViewModel>
{
}