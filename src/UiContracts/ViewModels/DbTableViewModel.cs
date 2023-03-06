using System.Diagnostics.CodeAnalysis;

using Library.Data.SqlServer.Dynamics;
using Library.Validations;

namespace HanyCo.Infra.UI.ViewModels;

public sealed class DbTableViewModel : DbObjectViewModel
{
    public DbTableViewModel(string name, long objectId, string? schema)
        : base(name, objectId, schema)
    {
    }

    [return: NotNull]
    public static DbTableViewModel FromDbObjectViewModel([DisallowNull] DbObjectViewModel viewModel)
    {
        Check.IfArgumentNotNull(viewModel);
        Check.IfArgumentNotNull(viewModel.Name);
        return viewModel is DbTableViewModel x ? x : new(viewModel.Name, viewModel.ObjectId, viewModel.Schema);
    }

    [return: NotNull]
    public static DbTableViewModel FromDbTable([DisallowNull] Table table)
    {
        Check.IfArgumentNotNull(table);
        return new DbTableViewModel(table.Name, table.Id, table.Schema);
    }
}