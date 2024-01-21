using Contracts.ViewModels;

using HanyCo.Infra.CodeGeneration.FormGenerator.Bases;

using Library.Interfaces;

namespace Contracts.Services;

public interface IApiCodingService : IBusinessService, ICodeGenerator<ApiCodingViewModel, ApiCodingArgs>
{
}

public sealed record ApiCodingArgs(bool generateGetAll, bool generateGetById, bool generateInsert, bool generateUpdate, bool generateDelete);