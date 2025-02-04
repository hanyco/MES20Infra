﻿using HanyCo.Infra.UI.ViewModels;

using Library.CodeGeneration.Models;
using Library.Collections;
using Library.Interfaces;

namespace HanyCo.Infra.UI.Services;

public interface ICqrsCodeGeneratorService : IService
{
    IEnumerable<GenerateAllCqrsCodesResultItem> GenerateAllCodes(CqrsCqrsGenerateCodesParams parametes, CqrsCodeGenerateCodesConfig? config = null);

    Task<Codes> GenerateCodeAsync(CqrsViewModelBase viewModel, CqrsCodeGenerateCodesConfig? config = null);

    Codes GenerateCreateCode(in CqrsCodeGenerateCrudParams parametes);

    Codes GenerateDeleteCode(in CqrsCodeGenerateCrudParams parametes);

    Codes GenerateGetAllCode(in CqrsCodeGenerateCrudParams parametes);

    Codes GenerateGetByIdCode(in CqrsCodeGenerateCrudParams parametes);

    Codes GenerateUpdateCode(in CqrsCodeGenerateCrudParams parametes);

    Task SaveToDatabaseAsync(CqrsCqrsGenerateCodesParams parametes, CqrsCodeGenerateCodesConfig config);

    Task SaveToDiskAsync(CqrsViewModelBase viewModel, string path, CqrsCodeGenerateCodesConfig? config = null);
}

public record CqrsCodeGenerateCrudParams(in Node<DbObjectViewModel> Table, in string CqrsNameSpace, in string DtoNameSpace);

public sealed record CqrsCqrsGenerateCodesParams(in string? EntityName, in Node<DbObjectViewModel> Table, in string CqrsNameSpace, in string DtoNameSpace)
    : CqrsCodeGenerateCrudParams(Table, CqrsNameSpace, DtoNameSpace);

public sealed record CqrsCodeGenerateCodesConfig(bool ShouldGenerateGetAll = true,
    bool ShouldGenerateGetById = true,
    bool ShouldGenerateCreate = true,
    bool ShouldGenerateUpdate = true,
    bool ShouldGenerateDelete = true);

public sealed record GenerateAllCqrsCodesResultItem(in string Name, in Codes Codes);