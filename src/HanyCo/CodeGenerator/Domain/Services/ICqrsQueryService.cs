﻿using HanyCo.Infra.CodeGen.Domain.ViewModels;
using HanyCo.Infra.Internals.Data.DataSources;

using Library.Interfaces;
using Library.Results;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface ICqrsQueryService : IBusinessService, IAsyncCrud<CqrsQueryViewModel>, IAsyncCreator<CqrsQueryViewModel>, IResetChanges
{
    Task<bool> AnyByName(string name);
    Task<Result> DeleteById(long id, bool persist = true, CancellationToken token = default);

    Task<CqrsQueryViewModel> FillByDbEntity(CqrsQueryViewModel @this, long dbQueryId, string? moduleName = null, string? paramDtoName = null, string? resultDtoName = null, CancellationToken token = default);

    CqrsQueryViewModel FillByDbEntity(CqrsQueryViewModel @this, CqrsSegregate dbQuery, string? moduleName = null, string? paramDtoName = null, string? resultDtoName = null);

    Task<CqrsQueryViewModel> FillViewModelAsync(CqrsQueryViewModel model,
        string? moduleName = null,
        string? paramDtoName = null,
        string? resultDtoName = null, CancellationToken token = default);

    Task<IReadOnlyList<CqrsQueryViewModel>> GetQueriesByDtoIdAsync(long dtoId, CancellationToken token = default);
}