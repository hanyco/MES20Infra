﻿using HanyCo.Infra.CodeGen.Contracts.ViewModels;

using Library.Collections;
using Library.DesignPatterns.Markers;
using Library.Extensions.Options;
using Library.Interfaces;
using Library.Threading.MultistepProgress;

namespace HanyCo.Infra.CodeGen.Contracts.Services;

public interface IDbTableService : IService
{
    Task<IEnumerable<DbColumnViewModel>> GetColumnsAsync(string connectionString, string tableName, CancellationToken token = default);

    Task<IReadOnlyList<Node<DbObjectViewModel>>> GetTablesTreeViewItemAsync(GetTablesTreeViewItemOptions options, CancellationToken token = default);
}

[Immutable]
public readonly struct GetTablesTreeViewItemOptions(string connectionString, bool gatherColumns = false, IProgressReport? reporter = null) : IOptions
{
    public string ConnectionString { get; } = connectionString;
    public bool GatherColumns { get; } = gatherColumns;
    public IProgressReport? Reporter { get; } = reporter;

    public void Deconstruct(out string connectionString, out bool gatherColumns, out IProgressReport? reporter)
        => (connectionString, gatherColumns, reporter) = (this.ConnectionString, this.GatherColumns, this.Reporter);
}