using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace Contracts.Services;

public interface IAdvancedSearchService : IBusinessService, ICodeGenerator<AdvancedSearchViewModel>
{
}