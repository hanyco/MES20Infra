using HanyCo.Infra.CodeGen.Domain.ViewModels;

using Library.Collections;
using Library.DesignPatterns.Markers;
using Library.Interfaces;
using Library.Threading.MultistepProgress;

namespace HanyCo.Infra.CodeGen.Domain.Services;

public interface IDbTableService : IBusinessService
{
    Task<IEnumerable<DbColumnViewModel>> GetColumns(string tableName, CancellationToken token = default);

    Task<DbColumnViewModel> GetIdentityColumn(string tableName, CancellationToken token = default);

    Task<IReadOnlyList<Node<DbObjectViewModel>>> GetTablesTree(GetTablesTreeViewItemOptions options, CancellationToken token = default);
}

[Immutable]
public readonly record struct GetTablesTreeViewItemOptions(bool GatherColumns = false, IProgressReport? Reporter = null);