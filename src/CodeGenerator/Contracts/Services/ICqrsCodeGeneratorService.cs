using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;
using HanyCo.Infra.UI.ViewModels;

using Library.Collections;
using Library.Interfaces;

namespace Contracts.Services;

public interface ICqrsCodeGeneratorService : IBusinessService, ICodeGenerator<CqrsViewModelBase, CqrsCodeGenerateCodesConfig>
{
}

public sealed record CqrsCodeGenerateCodesConfig(
    bool ShouldGenerateGetAll = true,
    bool ShouldGenerateGetById = true,
    bool ShouldGenerateCreate = true,
    bool ShouldGenerateUpdate = true,
    bool ShouldGenerateDelete = true,
    bool ShouldGenerateValidators = true);