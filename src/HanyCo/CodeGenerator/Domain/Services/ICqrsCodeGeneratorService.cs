using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Collections;
using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface ICqrsCodeGeneratorService : IBusinessService, ICodeGenerator<CqrsViewModelBase, CqrsCodeGenerateCodesConfig>;

public sealed record CqrsCodeGenerateCodesConfig(
    bool ShouldGenerateParams = true,
    bool ShouldGenerateHandler = true,
    bool ShouldGenerateResult = true,
    bool AddProperties = true,
    bool InitPropsInCtor = true,
    bool CreateDefaultCtor = true,
    string? HandleMethodBodyCode = null);