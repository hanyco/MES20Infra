using Contracts.ViewModels;

using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Collections;
using Library.Interfaces;
using Library.Results;

namespace Contracts.Services;

public interface ICqrsCodeGeneratorService : IBusinessService//, IAsyncCqrsCodeGeneratorService
{
    //IEnumerable<GenerateAllCqrsCodesResultItem> GenerateAllCodes(CqrsGenerateCodesParams parameters, CqrsCodeGenerateCodesConfig? config = null);

    Task<Result<Codes>> GenerateCodeAsync(CqrsViewModelBase viewModel, CqrsCodeGenerateCodesConfig? config = null, CancellationToken token = default);

    Task SaveToDatabaseAsync(CqrsGenerateCodesParams parameters, CqrsCodeGenerateCodesConfig config, CancellationToken token = default);
}

public record CqrsCodeGenerateCrudParams(in Node<DbObjectViewModel> Table, in string CqrsNameSpace, in string DtoNameSpace);

public sealed record CqrsGenerateCodesParams(in string? EntityName, in Node<DbObjectViewModel> Table, in string CqrsNameSpace, in string DtoNameSpace)
    : CqrsCodeGenerateCrudParams(Table, CqrsNameSpace, DtoNameSpace);

public sealed record CqrsCodeGenerateCodesConfig(
    bool ShouldGenerateGetAll = true,
    bool ShouldGenerateGetById = true,
    bool ShouldGenerateCreate = true,
    bool ShouldGenerateUpdate = true,
    bool ShouldGenerateDelete = true);

//public sealed record GenerateAllCqrsCodesResultItem(in string Name, in Codes Codes);