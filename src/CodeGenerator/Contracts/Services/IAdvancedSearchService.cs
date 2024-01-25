using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IAdvancedSearchService : IBusinessService, ICodeGenerator<AdvancedSearchViewModel>
{
}