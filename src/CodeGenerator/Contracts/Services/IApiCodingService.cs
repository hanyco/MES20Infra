using HanyCo.Infra.CodeGen.Contracts.ViewModels;
using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IApiCodeGenerator : IBusinessService, ICodeGenerator<ApiCodingViewModel, ApiCodingArgs>
{
}

public sealed record ApiCodingArgs(
    bool GenerateGetAll = true,
    bool GenerateGetById = true,
    bool GeneratePost = true,
    bool GeneratePut = true,
    bool GenerateDelete = true);