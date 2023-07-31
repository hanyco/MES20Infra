using HanyCo.Infra.UI.ViewModels;

using Library.Collections;
using Library.Interfaces;
using Library.Threading.MultistepProgress;

namespace HanyCo.Infra.UI.Services;

public interface IDbTableService : IService
{
    Task<IReadOnlyList<Node<DbObjectViewModel>>> GetTablesTreeViewItemAsync(IProgressReport? reporter, string connectionString, CancellationToken token = default);
}