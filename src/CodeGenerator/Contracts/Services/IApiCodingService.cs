using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace Contracts.Services;

public interface IApiCodingService : IBusinessService, ICodeGenerator<ApiCodingViewModel, ApiCodingArgs>
{
}

public sealed record ApiCodingArgs(
    bool GenerateGetAll = true,
    bool GenerateGetById = true,
    bool GeneratePost = true,
    bool GeneratePut = true,
    bool GenerateDelete = true);