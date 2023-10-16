using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.CodeGenerator.Models;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

public interface IDtoCodeService : IBusinessService, ICodeGenerator<DtoViewModel, GenerateCodesParameters>
{

}
